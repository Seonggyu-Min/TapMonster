public class StatManager
{
    private StatBuilderService _statBuilderService;
    private IStatContributor[] _statContributors;
    private GameConfigSO _gameConfigSO;

    private bool _dirty = true;
    private PlayerStatSnapshot _cached;

    public StatManager(StatBuilderService statBuilderService, IStatContributor[] statContributors, GameConfigSO gameConfigSO)
    {
        _statBuilderService = statBuilderService;
        _statContributors = statContributors;
        _gameConfigSO = gameConfigSO;
    }

    public void MarkDirty()
    {
        _dirty = true;
    }

    public PlayerStatSnapshot GetOrBuildSnapshot()
    {
        if (_dirty)
        {
            _cached = _statBuilderService.BuildSnapshot(_gameConfigSO, _statContributors);
            _dirty = false;
        }
        return _cached;
    }

    public PlayerStatSnapshot ForceRebuildSnapshot()
    {
        _cached = _statBuilderService.BuildSnapshot(_gameConfigSO, _statContributors);
        _dirty = false;
        return _cached;
    }
}
