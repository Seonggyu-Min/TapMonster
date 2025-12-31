using Cysharp.Threading.Tasks;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;



public class RTDBService : Singleton<RTDBService>
{
    #region Fields and Properties

    private const int DefaultTimeoutMs = 3000;

    private FirebaseDatabase DB
    {
        get
        {
            if (FirebaseManager.Instance == null)
            {
                throw new InvalidOperationException("FirebaseManager instance is null");
            }
            if (!FirebaseManager.Instance.IsReady)
            {
                throw new InvalidOperationException("FirebaseManager is not ready yet. Please wait until it is initialized.");
            }

            return FirebaseManager.Instance.Database;
        }
    }

    #endregion


    #region Private Methods
    // 순차 실행, 동시 실행, 타임아웃, 리트라이 등을 구현할 수 있기에 분리해둠

    // ------- 기본 읽기/쓰기 -------

    // 지정된 경로의 데이터를 가져옵니다.
    private async Task<DataSnapshot> GetAsync(string path)
    {
        var snap = await DB.GetReference(path).GetValueAsync();
        return snap;
    }

    // 지정된 경로의 데이터를 덮어씁니다.
    private async Task SetAsync(object value, string path)
    {
        await DB.GetReference(path).SetValueAsync(value);
    }

    // 여러 경로를 동시에 업데이트합니다.
    private async Task UpdateAsync(Dictionary<string, object> updatesByPath)
    {
        await DB.RootReference.UpdateChildrenAsync(updatesByPath);
    }

    // 해당 경로의 노드를 삭제합니다.
    private async Task RemoveAsync(string path)
    {
        await DB.GetReference(path).RemoveValueAsync();
    }

    // ------- 트랜잭션 처리 -------

    // 지정된 경로에서 트랜잭션을 실행합니다.
    private Task<DataSnapshot> RunTransactionAsync(Func<MutableData, TransactionResult> handler, string path, bool fireLocalEvents = false)
    {
        return DB.GetReference(path).RunTransaction(handler, fireLocalEvents);
    }

    // 지정된 경로의 숫자를 증감시킵니다.
    private Task<DataSnapshot> IncrementAsync(long delta, string path)
    {
        return RunTransactionAsync(mutable =>
        {
            long current = 0;
            try
            {
                if (mutable.Value != null)
                {
                    current = Convert.ToInt64(mutable.Value);
                }
            }
            catch
            {
                // 파싱 실패 시 0으로 간주
            }

            mutable.Value = current + delta;
            return TransactionResult.Success(mutable);
        }, path);
    }

    #endregion


    #region Log Methods

    private void LogCanceled(string op) => this.PrintLog($"{op} Canceled", LogCategory.Firebase, LogType.Warning);
    private void LogTimeout(string op) => this.PrintLog($"{op} Timeout", LogCategory.Firebase, LogType.Warning);
    private void LogError(string op, Exception ex) => this.PrintLog($"{op} Error: {ex}", LogCategory.Firebase, LogType.Error);
    private void LogSuccess(string op, string extra = null)
    {
        if (string.IsNullOrEmpty(extra)) this.PrintLog($"{op} Success", LogCategory.Firebase, LogType.Log);
        else this.PrintLog($"{op} Success: {extra}", LogCategory.Firebase, LogType.Log);
    }

    #endregion


    #region UniTask Helpers

    private UniTask ApplyTimeout(UniTask task, int timeoutMs)
    {
        if (timeoutMs <= 0) return task;
        return task.Timeout(TimeSpan.FromMilliseconds(timeoutMs));
    }

    private UniTask<T> ApplyTimeout<T>(UniTask<T> task, int timeoutMs)
    {
        if (timeoutMs <= 0) return task;
        return task.Timeout(TimeSpan.FromMilliseconds(timeoutMs));
    }

    private async UniTask RunDbOp(string op, Func<CancellationToken, UniTask> body, CancellationToken ct)
    {
        try
        {
            await body(ct);
        }
        catch (TimeoutException)
        {
            LogTimeout(op);
            throw;
        }
        catch (OperationCanceledException)
        {
            LogCanceled(op);
            throw;
        }
        catch (Exception e)
        {
            LogError(op, e);
            throw;
        }
    }

    private async UniTask<T> RunDbOp<T>(string op, Func<CancellationToken, UniTask<T>> body, CancellationToken ct)
    {
        try
        {
            return await body(ct);
        }
        catch (TimeoutException)
        {
            LogTimeout(op);
            throw;
        }
        catch (OperationCanceledException)
        {
            LogCanceled(op);
            throw;
        }
        catch (Exception e)
        {
            LogError(op, e);
            throw;
        }
    }

    #endregion


    #region UniTask Public API

    public UniTask WaitFirebaseReadyAsync(CancellationToken ct = default)
    {
        return UniTask.WaitUntil(
            () => FirebaseManager.Instance != null && FirebaseManager.Instance.IsReady,
            cancellationToken: ct
        );
    }

    public UniTask<DataSnapshot> GetUniTaskAsync(string path, CancellationToken ct = default, int timeoutMs = DefaultTimeoutMs)
    {
        string op = $"Get ({path})";

        return RunDbOp(op, async token =>
        {
            UniTask<DataSnapshot> ut = GetAsync(path)
                .AsUniTask()
                .AttachExternalCancellation(token);

            ut = ApplyTimeout(ut, timeoutMs);

            DataSnapshot snap = await ut;
            LogSuccess(op, snap.Exists ? snap.GetRawJsonValue() : "null");
            return snap;
        }, ct);
    }

    public UniTask SetUniTaskAsync(string path, object value, CancellationToken ct = default, int timeoutMs = DefaultTimeoutMs)
    {
        string op = $"Set ({path})";

        return RunDbOp(op, async token =>
        {
            UniTask ut = SetAsync(value, path)
                .AsUniTask()
                .AttachExternalCancellation(token);

            ut = ApplyTimeout(ut, timeoutMs);

            await ut;
            LogSuccess(op);
        }, ct);
    }

    public UniTask UpdateUniTaskAsync(Dictionary<string, object> updatesByPath, CancellationToken ct = default, int timeoutMs = DefaultTimeoutMs)
    {
        string op = "UpdateChildren";

        return RunDbOp(op, async token =>
        {
            UniTask ut = UpdateAsync(updatesByPath)
                .AsUniTask()
                .AttachExternalCancellation(token);

            ut = ApplyTimeout(ut, timeoutMs);

            await ut;
            LogSuccess(op);
        }, ct);
    }

    public UniTask RemoveUniTaskAsync(string path, CancellationToken ct = default, int timeoutMs = DefaultTimeoutMs)
    {
        string op = $"Remove ({path})";

        return RunDbOp(op, async token =>
        {
            UniTask ut = RemoveAsync(path)
                .AsUniTask()
                .AttachExternalCancellation(token);

            ut = ApplyTimeout(ut, timeoutMs);

            await ut;
            LogSuccess(op);
        }, ct);
    }

    public UniTask<DataSnapshot> RunTransactionUniTaskAsync(
        string path,
        Func<MutableData, TransactionResult> handler,
        bool fireLocalEvents = false,
        CancellationToken ct = default,
        int timeoutMs = DefaultTimeoutMs)
    {
        string op = $"Transaction ({path})";

        return RunDbOp(op, async token =>
        {
            UniTask<DataSnapshot> ut = RunTransactionAsync(handler, path, fireLocalEvents)
                .AsUniTask()
                .AttachExternalCancellation(token);

            ut = ApplyTimeout(ut, timeoutMs);

            DataSnapshot snap = await ut;
            LogSuccess(op, snap.Exists ? (snap.Value?.ToString() ?? "null") : "null");
            return snap;
        }, ct);
    }

    public UniTask<DataSnapshot> IncrementUniTaskAsync(string path, long delta, CancellationToken ct = default, int timeoutMs = DefaultTimeoutMs)
    {
        string op = $"Increment ({path}) delta={delta}";

        return RunDbOp(op, async token =>
        {
            UniTask<DataSnapshot> ut = IncrementAsync(delta, path)
                .AsUniTask()
                .AttachExternalCancellation(token);

            ut = ApplyTimeout(ut, timeoutMs);

            DataSnapshot snap = await ut;
            LogSuccess(op, snap.Exists ? (snap.Value?.ToString() ?? "null") : "null");
            return snap;
        }, ct);
    }

    #endregion
}
