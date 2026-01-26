using System;

public class BossTimerCoordinator
{
    #region Fields and Properties

    private readonly BossTimerService _bossTimerService;

    private ISaveMark _saveMark;
    private StageManager _stageManager;
    private IBossTimeLimitModifier[] _modifiers;

    private readonly StageConfigSO _stageConfigSO;

    private bool _hasPendingLoad;
    private int _pendingBossStage;
    private float _pendingRemaining;

    private bool _activated;
    private float _saveAccumulatedTime;


    public event Action<bool> OnVisibleChanged;          // bool: UI show/hide
    public event Action<float, float> OnTimeChanged;     // float: remaining, float: duration
    public event Action<int> OnTimeout;                  // int: bossStage, 일단 없어도 될 듯

    private const LogCategory CurrnentCategory = LogCategory.GameLogic;

    public BossTimerModel BossTimerModel => _bossTimerService.BossTimerModel;

    #endregion


    #region Cycle Management

    public BossTimerCoordinator(
        BossTimerService bossTimerService,
        StageConfigSO stageConfigSO
        )
    {
        _bossTimerService = bossTimerService;
        _stageConfigSO = stageConfigSO;
    }
    public void Initialize(
        ISaveMark saveMark,
        StageManager stageManager,
        IBossTimeLimitModifier[] modifiers
        )
    {
        _saveMark = saveMark;
        _stageManager = stageManager;
        _modifiers = modifiers;
    }
    public void Activate()
    {
        if (_activated) return;
        _activated = true;

        _stageManager.OnBossStageStarted += HandleBossStageStarted;
        _stageManager.OnBossStageEnded += HandleBossStageEnded;

        _bossTimerService.OnTimeout += HandleTimeout;

        // 기본은 숨김
        OnVisibleChanged?.Invoke(false);
        OnTimeChanged?.Invoke(0f, 0f);
    }
    public void Deactivate()
    {
        if (!_activated) return;
        _activated = false;

        _stageManager.OnBossStageStarted -= HandleBossStageStarted;
        _stageManager.OnBossStageEnded -= HandleBossStageEnded;

        _bossTimerService.OnTimeout -= HandleTimeout;

        _bossTimerService.Stop();
        OnVisibleChanged?.Invoke(false);
        OnTimeChanged?.Invoke(0f, 0f);
    }

    #endregion


    #region Public Methods

    // GameCoordinator에서 Update 호출
    // 만약 Tick해야되는 매니저가 많아지면 인터페이스 ITickable 같은 확장도 괜찮을 듯
    public void Tick(float deltaTime)
    {
        if (!_activated) return;
        if (!BossTimerModel.IsRunning) return;

        _bossTimerService.Tick(deltaTime);
        OnTimeChanged?.Invoke(BossTimerModel.RemainingSeconds, BossTimerModel.DurationSeconds);

        _saveAccumulatedTime += deltaTime;
        if (_saveAccumulatedTime >= 1f)
        {
            _saveAccumulatedTime = 0f;
            _saveMark.MarkDirty(SaveDirtyFlags.BossTimer);
            _saveMark.RequestSave();
        }
    }

    public void ApplyLoadedState(bool isRunning, int bossStage, float remainingSeconds)
    {
        if (!isRunning || remainingSeconds <= 0f)
        {
            _hasPendingLoad = false;
            _pendingBossStage = 0;
            _pendingRemaining = 0f;
            return;
        }

        _hasPendingLoad = true;
        _pendingBossStage = bossStage;
        _pendingRemaining = remainingSeconds;
    }

    #region API Methods For Boss Pattern

    /// <summary>
    /// 남은 시간 증감
    /// </summary>
    public void AddTime(float deltaSeconds)
    {
        if (!BossTimerModel.IsRunning) return;
        _bossTimerService.AddTime(deltaSeconds);
        OnTimeChanged?.Invoke(BossTimerModel.RemainingSeconds, BossTimerModel.DurationSeconds);
    }

    /// <summary>
    /// 남은 시간 강제 설정
    /// </summary>
    public void SetRemaining(float seconds)
    {
        if (!BossTimerModel.IsRunning) return;
        _bossTimerService.SetRemaining(seconds);
        OnTimeChanged?.Invoke(BossTimerModel.RemainingSeconds, BossTimerModel.DurationSeconds);
    }

    /// <summary>
    /// Duration 최대치 증감. Remaining도 동시에 증감됨 
    /// </summary>
    public void AddDuration(float deltaSeconds)
    {
        if (!BossTimerModel.IsRunning) return;
        BossTimerModel.AddDuration(deltaSeconds);
        OnTimeChanged?.Invoke(BossTimerModel.RemainingSeconds, BossTimerModel.DurationSeconds);
    }

    #endregion

    #endregion


    #region Private Methods

    private void HandleBossStageStarted(int stage)
    {
        this.PrintLog($"[BossTimer] HandleBossStageStarted stage={stage} " +
            $"pending={_hasPendingLoad} " +
            $"pendingStage={_pendingBossStage} " +
            $"rem={_pendingRemaining}", CurrnentCategory);

        float duration = BuildInitialDurationSeconds();
        _bossTimerService.Start(stage, duration);

        if (_hasPendingLoad)
        {
            // 같은 보스 스테이지일 때만 적용(안전장치)
            if (_pendingBossStage == stage)
            {
                _bossTimerService.SetRemaining(_pendingRemaining);
            }

            _hasPendingLoad = false;
            _pendingBossStage = 0;
            _pendingRemaining = 0f;
        }

        OnVisibleChanged?.Invoke(true);
        OnTimeChanged?.Invoke(BossTimerModel.RemainingSeconds, BossTimerModel.DurationSeconds);
    }

    private void HandleBossStageEnded(int stage)
    {
        _bossTimerService.Stop();

        OnVisibleChanged?.Invoke(false);
        OnTimeChanged?.Invoke(0f, 0f);
    }

    private void HandleTimeout(int bossStage)
    {
        // 타이머 정지
        _bossTimerService.Stop();

        OnVisibleChanged?.Invoke(false);
        OnTimeChanged?.Invoke(0f, 0f);

        // StageManager에서 Stage 직접 처리
        _stageManager.RollbackFromBossTimeout();
        OnTimeout?.Invoke(bossStage);
    }

    private float BuildInitialDurationSeconds()
    {
        float seconds = _stageConfigSO.BossLimitTimer;

        if (_modifiers != null)
        {
            for (int i = 0; i < _modifiers.Length; i++)
            {
                IBossTimeLimitModifier m = _modifiers[i];
                if (m == null) continue;
                seconds = m.Modify(seconds);
            }
        }

        if (seconds < 1f) seconds = 1f;
        return seconds;
    }

    #endregion
}
