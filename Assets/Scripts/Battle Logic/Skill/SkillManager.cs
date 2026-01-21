using System;
using UnityEngine;


public class SkillManager : IStatContributor, IStatModifier
{
    private SkillService _skillService;
    private GameConfigSO _gameConfigSO;
    private PurchaseManager _purchaseManager;
    private ISaveMark _saveMark;

    public event Action<int, int> OnSkillLevelChanged
    {
        add => _skillService.OnSkillLevelChanged += value;
        remove => _skillService.OnSkillLevelChanged -= value;
    }

    public event Action<SkillUseEvent> OnSkillUsed  // UI
    {
        add => _skillService.OnSkillUsed += value;
        remove => _skillService.OnSkillUsed -= value;
    }

    public event Action<int, SkillUseResult> OnSkillUseFailed // UI
    {
        add => _skillService.OnSkillUseFailed += value;
        remove => _skillService.OnSkillUseFailed -= value;
    }


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


    public int GetLevel(int skillId)
    {
        return _skillService.GetLevel(skillId);
    }

    public Cost GetNextCost(int skillId)
    {
        return new Cost(
            CurrencyId.Gold,
            _skillService.GetLevelUpCost(
                _gameConfigSO,
                skillId,
                _skillService.GetLevel(skillId) + 1));
    }

    public void TryLevelUpSkill(int skillId)
    {
        int nextLevel = _skillService.GetLevel(skillId) + 1;
        Cost cost = new Cost(
            CurrencyId.Gold,
            _skillService.GetLevelUpCost(
                _gameConfigSO,
                skillId,
                nextLevel));

        var result = _purchaseManager.TryPay(cost);
        if (result != PurchaseResult.Success) return;

        _skillService.AddLevel(skillId, +1);

        _saveMark.MarkDirty(SaveDirtyFlags.Skill);
        _saveMark.RequestSave();
    }

    public SkillUseResult TryUseSkill(int skillId)
    {
        float now = Time.unscaledTime;

        SkillUseResult result = _skillService.TryUseSkill(skillId, _gameConfigSO, now);

        if (result == SkillUseResult.Success)
        {
            // 일단 쿨타임은 저장 안함
            //_saveMark.MarkDirty(SaveDirtyFlags.Skill);
            //_saveMark.RequestSave();
        }

        return result;
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
