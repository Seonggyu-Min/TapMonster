using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;


public class SaveLoadManager : ISaveMark
{
    #region Fields and Constructor

    private SaveLoadService _saveLoadService;
    private GameStateModel _gameStateModel;
    private SavePatchBuilder _savePatchBuilder;

    private string _uid;

    private SaveDirtyFlags _dirty = SaveDirtyFlags.None;
    private bool _saveRequested;

    private CancellationTokenSource _debounceCts;
    private CancellationTokenSource _throttleCts;
    private bool _throttleArmed;
    private float _saveDebounceSeconds;
    private readonly float _maxSaveIntervalSeconds;

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

    #endregion


    #region Public Methods

    /// <summary>
    /// 게임 최초 실행 시 전체 불러오기
    /// </summary>
    public async UniTask LoadAllAsync(CancellationToken ct)
    {
        SaveDataDTO dto = await _saveLoadService.LoadAsync(_uid, ct);
        _gameStateModel.ApplyToClient(dto);

        _dirty = SaveDirtyFlags.None;
        _saveRequested = false;

        CancelDebounce();
        CancelThrottle();
    }

    /// <summary>
    /// 저장할 항목 마크
    /// </summary>
    public void MarkDirty(SaveDirtyFlags flags)
    {
        _dirty |= flags;
    }

    /// <summary>
    /// 저장 요청
    /// </summary>
    public void RequestSave()
    {
        _saveRequested = true;

        // 이전 Debounce 취소 후 새로 시작
        CancelDebounce();
        _debounceCts = new();
        DebouncedFlushAsync(_debounceCts.Token).Forget();

        // Throttle 요청
        ArmThrottleIfNeeded();
    }

    /// <summary>
    /// 강제 저장
    /// </summary>
    public async UniTask ForceSaveAsync(CancellationToken ct)
    {
        CancelDebounce();
        CancelThrottle();

        if (_dirty == SaveDirtyFlags.None) return;

        await FlushCoreAsync(ct);
    }

    #endregion


    #region Private Methods

    // Throttle 시작
    private void ArmThrottleIfNeeded()
    {
        if (_throttleArmed) return; // 최대 대기 시간 이미 설정되어 return

        _throttleArmed = true;
        _throttleCts = new();
        ThrottledFlushAsync(_throttleCts.Token).Forget();
    }

    // Debounce 취소
    private void CancelDebounce()
    {
        _debounceCts?.Cancel();
        _debounceCts?.Dispose();
        _debounceCts = null;
    }

    // Throttle 취소
    private void CancelThrottle()
    {
        _throttleCts?.Cancel();
        _throttleCts?.Dispose();
        _throttleCts = null;
        _throttleArmed = false;
    }

    // 마크된 플래그 기준 실제 저장
    private async UniTask FlushCoreAsync(CancellationToken ct)
    {
        // 요청 상태 해제
        _saveRequested = false;

        SaveDirtyFlags flushingDirty = _dirty;
        _dirty = SaveDirtyFlags.None;

        var updates = _savePatchBuilder.BuildPatch(_uid, flushingDirty, _gameStateModel);
        await _saveLoadService.PatchAsync(_uid, updates, ct);
    }

    // 예약된 저장 실행
    private async UniTask TryFlushAsync(CancellationToken ct)
    {
        if (!_saveRequested) return;
        if (_dirty == SaveDirtyFlags.None)
        {
            _saveRequested = false;
            return;
        }

        await FlushCoreAsync(ct);

        // 저장 했으니까 Throttle 해제
        CancelThrottle();
    }

    // Debounce N초 후 저장 시도
    private async UniTaskVoid DebouncedFlushAsync(CancellationToken ct)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_saveDebounceSeconds), cancellationToken: ct);
            await TryFlushAsync(ct);
        }
        catch (OperationCanceledException) { }
        catch (Exception e) { this.PrintLog($"Debounce 예외: {e}", CurrentCategory, LogType.Error); }
    }

    private async UniTaskVoid ThrottledFlushAsync(CancellationToken ct)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_maxSaveIntervalSeconds), cancellationToken: ct);

            // max wait 도달 후 강제 flush 시도
            await TryFlushAsync(ct);

            // 여기까지 왔으면 throttle 성공
            CancelThrottle();
        }
        catch (OperationCanceledException) { }
        catch (Exception e) { this.PrintLog($"Throttle 예외: {e}", CurrentCategory, LogType.Error); }
    }

    #endregion
}
