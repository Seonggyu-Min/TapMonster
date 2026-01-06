public class RelicManager : IStatContributor
{
    private RelicService _relicService;
    private RelicGachaService _relicGachaService;
    private GameConfigSO _gameConfigSO;

    public RelicManager(RelicService relicService, RelicGachaService relicGachaService, GameConfigSO gameConfigSO)
    {
        _relicService = relicService;
        _relicGachaService = relicGachaService;
        _gameConfigSO = gameConfigSO;
    }


    public void Contribute(ref PlayerStatBuildContext buildContext)
    {
        throw new System.NotImplementedException();
    }
}
