public class SkillManager : IStatContributor, IStatModifier
{
    private SkillService _skillService;
    private GameConfigSO _gameConfigSO;

    public SkillManager(SkillService skillService, GameConfigSO gameConfigSO)
    {
        _skillService = skillService;
        _gameConfigSO = gameConfigSO;
    }

    public void Contribute(ref PlayerStatBuildContext buildContext)
    {
        throw new System.NotImplementedException();
    }

    public void Modify(ref DamageContext damageContext)
    {
        throw new System.NotImplementedException();
    }
}
