public class StatManager
{
    private readonly StatBuilderService _statBuilderService;
    private readonly GameConfigSO _gameConfigSO;
    private IStatContributor[] _statContributors;

    private bool _dirty = true;
    private PlayerStatSnapshot _cached;

    public StatManager(StatBuilderService statBuilderService, GameConfigSO gameConfigSO)
    {
        _statBuilderService = statBuilderService;
        _gameConfigSO = gameConfigSO;
    }
    public void Initialize(IStatContributor[] statContributors)
    {
        _statContributors = statContributors;
    }
    public void Activate() { /* no op*/ }
    public void Deactivate() { /* no op*/ }

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
