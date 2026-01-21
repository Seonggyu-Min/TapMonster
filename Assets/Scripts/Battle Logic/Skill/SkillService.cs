using System;
using System.Collections.Generic;

public class SkillService
{
    private SkillModel _skillModel;
    private SkillCooldownModel _skillCooldownModel;
    private SkillSlotModel _skillSlotModel;

    public event Action<SkillUseEvent> OnSkillUsed;
    public event Action<int, SkillUseResult> OnSkillUseFailed;

    public event Action<int, int> OnSkillLevelChanged
    {
        add => _skillModel.OnSkillLevelChanged += value;
        remove => _skillModel.OnSkillLevelChanged -= value;
    }

    public event Action<SkillSlotChangeKind> OnSkillSlotChanged
    {
        add => _skillSlotModel.OnSkillSlotChanged += value;
        remove => _skillSlotModel.OnSkillSlotChanged -= value;
    }

    public event Action<SkillSlotChangeEvent> OnSkillSlotChangeEvent
    {
        add => _skillSlotModel.OnSkillSlotChangeEvent += value;
        remove => _skillSlotModel.OnSkillSlotChangeEvent -= value;
    }

    public SkillService(
        SkillModel skillModel,
        SkillCooldownModel skillCooldownModel,
        SkillSlotModel skillSlotModel
        )
    {
        _skillModel = skillModel;
        _skillCooldownModel = skillCooldownModel;
        _skillSlotModel = skillSlotModel;
    }

    public int GetLevel(int skillId) => _skillModel.GetLevel(skillId);
    public void AddLevel(int skillId, int delta) => _skillModel.AddLevel(skillId, delta);


    public bool CanUseSkill(int skillId, float now)
        => _skillCooldownModel.CanUse(skillId, now);

    public float GetCooldownRemaining(int skillId, float now)
        => _skillCooldownModel.GetRemaining(skillId, now);


    public SkillUseResult TryUseSkill(int skillId, GameConfigSO config, float now)
    {
        if (!config.SkillConfigSO.TryGet(skillId, out var def))
        {
            OnSkillUseFailed?.Invoke(skillId, SkillUseResult.InvalidSkillId);
            return SkillUseResult.InvalidSkillId;
        }

        int level = _skillModel.GetLevel(skillId);
        if (level <= 0)
        {
            OnSkillUseFailed?.Invoke(skillId, SkillUseResult.LevelZero);
            return SkillUseResult.LevelZero;
        }

        if (def.Kind != SkillKind.Active)
        {
            OnSkillUseFailed?.Invoke(skillId, SkillUseResult.NotActive);
            return SkillUseResult.NotActive;
        }

        if (!_skillCooldownModel.CanUse(skillId, now))
        {
            OnSkillUseFailed?.Invoke(skillId, SkillUseResult.Cooldown);
            return SkillUseResult.Cooldown;
        }


        float cd = def.CooldownSeconds;
        _skillCooldownModel.StartCooldown(skillId, now, cd);

        var useEvent = new SkillUseEvent(skillId, level, cd);
        OnSkillUsed?.Invoke(useEvent);
        return SkillUseResult.Success;
    }

    public float GetSkillCooldownSeconds(GameConfigSO config, int skillId)
    {
        if (!config.SkillConfigSO.TryGet(skillId, out var def)) return 0f;
        return def.CooldownSeconds;
    }

    public BigNumber GetLevelUpCost(GameConfigSO config, int skillId, int nextLevel)
    {
        if (!config.SkillConfigSO.TryGet(skillId, out var def))
            return BigNumber.One;

        double mul = Math.Pow(def.CostGrowth, Math.Max(0, nextLevel - 1));
        return def.BaseCost * mul;
    }

    public void ApplyPassiveToStat(ref PlayerStatBuildContext ctx, GameConfigSO config)
    {
        foreach (var kv in _skillModel.SkillLevels)
        {
            int id = kv.Key;
            int level = kv.Value;
            if (level <= 0) continue;

            if (!config.SkillConfigSO.TryGet(id, out var def))
                continue;

            if (def.Kind != SkillKind.Passive) continue;

            double v = def.PassiveValuePerLevel * level;

            switch (def.PassiveType)
            {
                case SkillPassiveType.ManualAdditiveDamagePercent:
                    ctx.ManualAdditiveDamagePercent += (float)v;
                    break;

                case SkillPassiveType.AutoAdditiveDamagePercent:
                    ctx.AutoAdditiveDamagePercent += (float)v;
                    break;

                case SkillPassiveType.ManualDamageMultiplier:
                    ctx.ManualDamageMultiplier *= (float)v;
                    break;

                case SkillPassiveType.AutoDamageMultiplier:
                    ctx.AutoDamageMultiplier *= (float)v;
                    break;

                case SkillPassiveType.FinalAllDamageMultiplier:
                    ctx.FinalAllDamageMultiplier *= (float)v;
                    break;

                case SkillPassiveType.ManualCriticalChance:
                    ctx.ManualCriticalChance += (float)v;
                    break;

                case SkillPassiveType.AutoCriticalChance:
                    ctx.AutoCriticalChance += (float)v;
                    break;
            }
        }
    }

    public void ApplyActiveToDamage(ref CalculatingDamageContext dmg, GameConfigSO config)
    {
        foreach (var kv in _skillModel.SkillLevels)
        {
            int id = kv.Key;
            int level = kv.Value;
            if (level <= 0) continue;

            if (!config.SkillConfigSO.TryGet(id, out var def))
                continue;

            if (def.Kind != SkillKind.Active) continue;

            double mul = 1.0 + def.ActiveMulPerLevel * level;

            switch (def.ActiveType)
            {
                case SkillActiveType.ManualDamageMul:
                    if (dmg.DamageSource == DamageSource.Manual)
                        dmg.DamageMultiplier *= (float)mul;
                    break;

                case SkillActiveType.AutoDamageMul:
                    if (dmg.DamageSource == DamageSource.Auto)
                        dmg.DamageMultiplier *= (float)mul;
                    break;
            }
        }
    }

    public IReadOnlyList<int> Inventory => _skillSlotModel.Inventory;
    public int GetEquipped(int index) => _skillSlotModel.GetEquipped(index);
    public int GetInventory(int index) => _skillSlotModel.GetInventory(index);
    public void SwapEquipped(int a, int b) => _skillSlotModel.SwapEquipped(a, b);
    public void ReplaceEquipped(int index, int skillId) => _skillSlotModel.ReplaceEquipped(index, skillId);
    public void SetInitial(List<int> inventory, int[] equipped = null) => _skillSlotModel.SetInitial(inventory, equipped);
}


public enum SkillUseResult
{
    Success,            // 성공
    InvalidSkillId,     // 잘못된 스킬 ID
    LevelZero,          // 레벨 0
    Cooldown,           // 쿨타임 중
    NotActive,          // 액티브 스킬이 아님
}


public readonly struct SkillUseEvent
{
    public readonly int SkillId;
    public readonly int Level;
    public readonly float CooldownSeconds;

    public SkillUseEvent(int skillId, int level, float cooldownSeconds)
    {
        SkillId = skillId;
        Level = level;
        CooldownSeconds = cooldownSeconds;
    }
}