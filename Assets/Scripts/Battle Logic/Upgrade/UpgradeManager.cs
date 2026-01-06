public class UpgradeManager : IStatContributor
{
    private UpgradeService _upgradeService;
    private GameConfigSO _gameConfigSO;

    public UpgradeManager(UpgradeService upgradeService, GameConfigSO gameConfigSO)
    {
        _upgradeService = upgradeService;
        _gameConfigSO = gameConfigSO;
    }

    public void Contribute(ref PlayerStatBuildContext buildContext)
    {
        throw new System.NotImplementedException();
    }
}
