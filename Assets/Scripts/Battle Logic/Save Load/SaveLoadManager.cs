using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;


public class SaveLoadManager : ISaveMark
{
    private readonly SaveLoadService _saveLoadService;
    private readonly GameStateModel _gameStateModel;
    private readonly SavePatchBuilder _savePatchBuilder;
    private readonly string _uid;

    private SaveDirtyFlags _dirty = SaveDirtyFlags.None;
    private bool _saveRequested;

    private CancellationTokenSource _debounceCts;
    private CancellationTokenSource _throttleCts;
    private bool _throttleArmed;
    private readonly float _saveDebounceSeconds;
    private readonly float _maxSaveIntervalSeconds;

    private bool _isFlushing;
    private bool _flushAgainRequested;

    private const LogCategory CurrentCategory = LogCategory.GameLogic;

    public SaveLoadManager(
        string uid,
        GameStateModel gameState,
        SaveLoadService service,
        SavePatchBuilder savePatchBuilder,
        float saveDebounceSeconds = 2.0f,
        float maxSaveIntervalSeconds = 15.0f)
    {
        _uid = uid;
        _gameStateModel = gameState;
        _saveLoadService = service;
        _savePatchBuilder = savePatchBuilder;
        _saveDebounceSeconds = saveDebounceSeconds;
        _maxSaveIntervalSeconds = maxSaveIntervalSeconds;
    }
    public void Initialize() { /* no op*/ }
    public void Activate() { /* no op*/ }
    public void Deactivate() { /* no op*/ }


    public async UniTask LoadAllAsync(CancellationToken ct)
    {
        SaveDataDTO dto = await _saveLoadService.LoadAsync(_uid, ct);
        _gameStateModel.ApplyToClient(dto, out bool generated);

        if (generated)
        {
            MarkDirty(SaveDirtyFlags.SkillSlots);
            RequestSave();
        }

        _dirty = SaveDirtyFlags.None;
        _saveRequested = false;
    }

    public void MarkDirty(SaveDirtyFlags flags)
    {
        _dirty |= flags;
    }

    public void RequestSave()
    {
        _saveRequested = true;

        // flush 중이면 다시 하도록 표기
        if (_isFlushing)
        {
            _flushAgainRequested = true;
            return;
        }

        CancelDebounce();
        _debounceCts = new();
        DebouncedFlushAsync(_debounceCts.Token).Forget();

        ArmThrottleIfNeeded();
    }

    public async UniTask ForceSaveAsync(CancellationToken ct)
    {
        CancelDebounce();
        CancelThrottle();

        if (_dirty == SaveDirtyFlags.None) return;
        await FlushCoreAsync(ct);
    }

    private void ArmThrottleIfNeeded()
    {
        if (_throttleArmed) return;
        _throttleArmed = true;

        _throttleCts = new();
        ThrottledFlushAsync(_throttleCts.Token).Forget();
    }

    private void CancelDebounce()
    {
        _debounceCts?.Cancel();
        _debounceCts?.Dispose();
        _debounceCts = null;
    }

    private void CancelThrottle()
    {
        _throttleCts?.Cancel();
        _throttleCts?.Dispose();
        _throttleCts = null;
        _throttleArmed = false;
    }

    private async UniTask TryFlushAsync(CancellationToken ct)
    {
        if (!_saveRequested) return;

        if (_dirty == SaveDirtyFlags.None)
        {
            _saveRequested = false;
            return;
        }

        await FlushCoreAsync(ct);
        CancelThrottle();
    }

    private async UniTask FlushCoreAsync(CancellationToken ct)
    {
        if (_isFlushing) return;

        _isFlushing = true;
        _flushAgainRequested = false;

        try
        {
            // 요청 상태 해제
            _saveRequested = false;

            // flush 대상 스냅샷
            SaveDirtyFlags flushingDirty = _dirty;
            _dirty = SaveDirtyFlags.None;

            // updates 만들기
            var updates = _savePatchBuilder.BuildPatchFromGameState(_uid, flushingDirty, _gameStateModel);

            // 실제 패치
            await _saveLoadService.PatchAsync(_uid, updates, ct);
        }
        finally
        {
            _isFlushing = false;
        }

        // flush 중 다시 RequestSave 들어오면
        if (_flushAgainRequested)
        {
            _saveRequested = true;
            _flushAgainRequested = false;

            await TryFlushAsync(ct);
        }
    }

    private async UniTaskVoid DebouncedFlushAsync(CancellationToken ct)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_saveDebounceSeconds), cancellationToken: ct);
            await TryFlushAsync(ct);
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            this.PrintLog($"Debounce 예외: {e}", CurrentCategory, LogType.Error);
        }
    }

    private async UniTaskVoid ThrottledFlushAsync(CancellationToken ct)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_maxSaveIntervalSeconds), cancellationToken: ct);
            await TryFlushAsync(ct);
            CancelThrottle();
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            this.PrintLog($"Throttle 예외: {e}", CurrentCategory, LogType.Error);
        }
    }
}
