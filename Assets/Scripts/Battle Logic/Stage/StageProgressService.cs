public class StageProgressService
{
    private readonly StageModel _stageModel;

    public StageProgressService(StageModel stageModel)
    {
        _stageModel = stageModel;
    }

    public int CurrentStage => _stageModel.CurrentStage;
    public void SetStage(int stage) => _stageModel.SetStage(stage);
    public void AdvanceStage() => _stageModel.SetStage(_stageModel.CurrentStage + 1);
}
