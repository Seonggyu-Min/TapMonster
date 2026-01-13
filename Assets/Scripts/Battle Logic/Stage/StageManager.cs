public class StageManager
{
    private StageService _stageService;
    private GameConfigSO _gameConfigSO;
    private readonly ISaveMark _saveMark;

    public StageManager(StageService stageService, GameConfigSO gameConfigSO, ISaveMark saveMark)
    {
        _stageService = stageService;
        _gameConfigSO = gameConfigSO;
        _saveMark = saveMark;
    }

    public int CurrentStage => _stageService.CurrentStage;

    public void OnMonsterDefeated()
    {
        _stageService.AdvanceStage();

        _saveMark.MarkDirty(SaveDirtyFlags.Stage);
        _saveMark.RequestSave();
    }

    public BigNumber GetMonsterHpForCurrentStage()
    {
        int stage = _stageService.CurrentStage;

        // TODO: _config 기반 계산

        return BigNumber.One;
    }

    public BigNumber GetMonsterRewardForCurrentStage()
    {
        // TODO: _config 기반 계산

        return BigNumber.One;
    }
}
