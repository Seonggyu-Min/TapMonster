public class StageService
{
    private StageModel _stageModel;


    public StageService(StageModel stageModel)
    {
        _stageModel = stageModel;
    }


    public int CurrentStage => _stageModel.CurrentStage;

    public void SetStage(int stage)
    {
        _stageModel.SetStage(stage);
    }

    public void AdvanceStage()
    {
        _stageModel.SetStage(_stageModel.CurrentStage + 1);
    }
}
