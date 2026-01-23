using System;

public class StageIndexPresenter : IDisposable
{
    private StageIndexView _stageIndexView;
    private StageManager _stageManager;

    private bool _activated;

    public StageIndexPresenter(StageIndexView stageIndexView, StageManager stageManager)
    {
        _stageIndexView = stageIndexView;
        _stageManager = stageManager;
    }
    public void Initialize() { /*no op*/ }
    public void Activate()
    {
        if (_activated) return;
        _activated = true;

        _stageManager.OnStageChanged += OnStageChanged;
    }
    public void Dispose()
    {
        if (!_activated) return;
        _activated = false;

        _stageManager.OnStageChanged -= OnStageChanged;
    }

    private void OnStageChanged(int stageIndex)
    {
        _stageIndexView.SetStage(stageIndex);
    }
}
