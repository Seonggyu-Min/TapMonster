using System;

public class BossTimerService
{
    private readonly BossTimerModel _bossTimerModel;

    private bool _timeoutFired;

    public event Action<int> OnTimeout; // int: Stage

    public BossTimerService(BossTimerModel bossTimerModel)
    {
        _bossTimerModel = bossTimerModel;
    }

    public BossTimerModel BossTimerModel => _bossTimerModel;

    public void Start(int bossStage, float durationSeconds)
    {
        if (durationSeconds <= 0f) durationSeconds = 0.01f;

        _timeoutFired = false;
        _bossTimerModel.Start(bossStage, durationSeconds);
    }

    public void Stop()
    {
        _timeoutFired = false;
        _bossTimerModel.Stop();
    }

    public void Tick(float deltaTime)
    {
        if (!_bossTimerModel.IsRunning) return;
        if (_timeoutFired) return;

        float next = _bossTimerModel.RemainingSeconds - deltaTime;
        _bossTimerModel.SetRemaining(next);

        if (_bossTimerModel.RemainingSeconds <= 0f)
        {
            _timeoutFired = true;
            OnTimeout?.Invoke(_bossTimerModel.BossStage);
        }
    }


    /// <summary>
    /// 남은 시간 추가 및 감소
    /// </summary>
    public void AddTime(float deltaSeconds)
    {
        if (!_bossTimerModel.IsRunning || _timeoutFired) return;
        _bossTimerModel.AddRemaining(deltaSeconds);
    }

    /// <summary>
    /// 남은 시간 강제 설정
    /// </summary>
    public void SetRemaining(float seconds)
    {
        if (!_bossTimerModel.IsRunning || _timeoutFired) return;
        _bossTimerModel.SetRemaining(seconds);
    }
}
