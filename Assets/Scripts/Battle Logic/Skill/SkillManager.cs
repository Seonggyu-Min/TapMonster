using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 스킬 사용, 레벨 조회, 쿨다운 등을 관리합니다.
/// 직접 호출하여 스킬을 사용하지 않고, 입력은 CombatManager가 관리합니다.
/// </summary>
public class SkillManager : IStatContributor, IStatModifier
{
    private readonly SkillService _skillService;
    private readonly GameConfigSO _gameConfigSO;
    private PurchaseManager _purchaseManager;
    private ISaveMark _saveMark;

    private bool _activated;

    public event Action OnChanged;

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

    public event Action<SkillSlotChangeKind> OnSkillSlotChanged
    {
        add => _skillService.OnSkillSlotChanged += value;
        remove => _skillService.OnSkillSlotChanged -= value;
    }

    public event Action<SkillSlotChangeEvent> OnSkillSlotChangeEvent
    {
        add => _skillService.OnSkillSlotChangeEvent += value;
        remove => _skillService.OnSkillSlotChangeEvent -= value;
    }


    public SkillManager(
        SkillService skillService,
        GameConfigSO gameConfigSO
        )
    {
        _skillService = skillService;
        _gameConfigSO = gameConfigSO;
    }
    public void Initialize(ISaveMark saveMark, PurchaseManager purchaseManager)
    {
        _purchaseManager = purchaseManager;
        _saveMark = saveMark;
    }
    public void Activate()
    {
        if (_activated) return;
        _activated = true;

        _skillService.OnSkillLevelChanged += HandleSkillLevelChanged;
    }
    public void Deactivate()
    {
        if (!_activated) return;
        _activated = false;

        _skillService.OnSkillLevelChanged -= HandleSkillLevelChanged;
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

    public bool CanUseSkill(int skillId, float now)
        => _skillService.CanUseSkill(skillId, now);

    public float GetCooldownRemaining(int skillId, float now)
        => _skillService.GetCooldownRemaining(skillId, now);

    public float GetSkillCooldownSeconds(int skillId)
        => _skillService.GetSkillCooldownSeconds(_gameConfigSO, skillId);

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

    public void Modify(ref CalculatingDamageContext damageContext)
    {
        _skillService.ApplyActiveToDamage(ref damageContext, _gameConfigSO);
    }

    public IReadOnlyList<int> Inventory => _skillService.Inventory;
    public int GetEquipped(int index) => _skillService.GetEquipped(index);
    public int GetInventory(int index) => _skillService.GetInventory(index);
    public void SwapEquipped(int a, int b) => _skillService.SwapEquipped(a, b);
    public void ReplaceEquipped(int index, int skillId)
        => _skillService.ReplaceEquipped(index, skillId);
    public void SetInitial(List<int> inventory, int[] equipped = null)
        => _skillService.SetInitial(inventory, equipped);


    private void HandleSkillLevelChanged(int id, int level) => OnChanged?.Invoke();
}
