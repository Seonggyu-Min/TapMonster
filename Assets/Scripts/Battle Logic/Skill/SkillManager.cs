public class SkillManager : IStatContributor, IStatModifier
{
    private SkillService _skillService;
    private GameConfigSO _gameConfigSO;
    private PurchaseManager _purchaseManager;
    private ISaveMark _saveMark;

    public SkillManager(
        SkillService skillService,
        GameConfigSO gameConfigSO,
        PurchaseManager purchaseManager,
        ISaveMark saveMark
        )
    {
        _skillService = skillService;
        _gameConfigSO = gameConfigSO;
        _purchaseManager = purchaseManager;
        _saveMark = saveMark;
    }

    public void TryLevelUpSkill(int skillId)
    {
        int nextLevel = _skillService.GetLevel(skillId) + 1;

        // TODO: 스킬 강화 비용 config에서 계산
        BigNumber cost = BigNumber.One;

        var result = _purchaseManager.TryPay(new Cost(CurrencyId.Gold, cost));
        if (result != PurchaseResult.Success) return;

        _skillService.AddLevel(skillId, +1);

        _saveMark.MarkDirty(SaveDirtyFlags.Skill);
        _saveMark.RequestSave();
    }

    public void Contribute(ref PlayerStatBuildContext buildContext)
    {
        _skillService.ApplyPassiveToStat(ref buildContext, _gameConfigSO);
    }

    public void Modify(ref DamageContext damageContext)
    {
        _skillService.ApplyActiveToDamage(ref damageContext, _gameConfigSO);
    }
}
