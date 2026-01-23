using System;

public class StageIndexPresenter : IDisposable
{
    private StageIndexView _stageIndexView;
    private StageManager _stageManager;


    public StageIndexPresenter(StageIndexView stageIndexView, StageManager stageManager)
    {
        _stageIndexView = stageIndexView;
        _stageManager = stageManager;
    }
    public void Initialize() { /*no op*/ }
    public void Activate()
    {
        _stageManager.OnStageChanged += SetText;
    }
    public void Dispose()
    {
        _stageManager.OnStageChanged -= SetText;
    }

    private void SetText(int stageIndex)
    {
        _stageIndexView.SetStage(stageIndex);
    }
}
