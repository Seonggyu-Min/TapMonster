public class StatManager
{
    private readonly StatBuilderService _statBuilderService;
    private readonly GameConfigSO _gameConfigSO;
    private IStatContributor[] _statContributors;

    private bool _activated;

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
    public void Activate()
    {
        if (_activated) return;
        _activated = true;

        if (_statContributors == null) return;

        for (int i = 0; i < _statContributors.Length; i++)
        {
            IStatContributor c = _statContributors[i];
            if (c == null) continue;
            c.OnChanged += MarkDirty;
        }
    }
    public void Deactivate()
    {
        if (!_activated) return;
        _activated = false;

        if (_statContributors == null) return;

        for (int i = 0; i < _statContributors.Length; i++)
        {
            IStatContributor c = _statContributors[i];
            if (c == null) continue;
            c.OnChanged -= MarkDirty;
        }
    }

    public void MarkDirty() // public으로 안쓸 수도?
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
