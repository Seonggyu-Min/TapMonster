using System;

public class BossTimerPresenter : IDisposable
{
    private readonly BossTimerView _bossTimerView;
    private readonly BossTimerCoordinator _bossTimerCoordinator;

    private bool _activated;

    public BossTimerPresenter(
        BossTimerView bossTimerView,
        BossTimerCoordinator bossTimerCoordinator
        )
    {
        _bossTimerView = bossTimerView;
        _bossTimerCoordinator = bossTimerCoordinator;
    }
    public void Initialize() { /* no op */ }
    public void Activate()
    {
        if (_activated) return;
        _activated = true;

        _bossTimerCoordinator.OnVisibleChanged += HandleVisibleChanged;
        _bossTimerCoordinator.OnTimeChanged += HandleTimeChanged;

        // 초기 상태
        _bossTimerView.SetVisible(false);
        _bossTimerView.SetTimer(0f, 0f);
    }
    public void Dispose()
    {
        if (!_activated) return;
        _activated = false;

        _bossTimerCoordinator.OnVisibleChanged -= HandleVisibleChanged;
        _bossTimerCoordinator.OnTimeChanged -= HandleTimeChanged;
    }

    private void HandleVisibleChanged(bool visible)
    {
        _bossTimerView.SetVisible(visible);
    }

    private void HandleTimeChanged(float remaining, float duration)
    {
        _bossTimerView.SetTimer(remaining, duration);
    }
}
