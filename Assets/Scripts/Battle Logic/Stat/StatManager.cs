public class StatManager
{
    private StatBuilderService _statBuilderService;
    private IStatContributor[] _statContributors;
    private GameConfigSO _gameConfigSO;

    public StatManager(StatBuilderService statBuilderService, IStatContributor[] statContributors, GameConfigSO gameConfigSO)
    {
        _statBuilderService = statBuilderService;
        _statContributors = statContributors;
        _gameConfigSO = gameConfigSO;
    }
}
