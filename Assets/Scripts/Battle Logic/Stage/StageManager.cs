public class StageManager
{
    private StageService _stageService;
    private GameConfigSO _gameConfigSO;


    public StageManager(StageService stageService, GameConfigSO gameConfigSO)
    {
        _stageService = stageService;
        _gameConfigSO = gameConfigSO;
    }
}
