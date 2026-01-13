using System;

public class SkillService
{
    private SkillModel _skillModel;

    public SkillService(SkillModel skillModel)
    {
        _skillModel = skillModel;
    }

    public int GetLevel(int skillId) => _skillModel.GetLevel(skillId);
    public void AddLevel(int skillId, int delta) => _skillModel.AddLevel(skillId, delta);

    public BigNumber GetLevelUpCost(GameConfigSO config, int skillId, int nextLevel)
    {
        if (!config.SkillConfigSO.TryGet(skillId, out var def))
            return BigNumber.One;

        double mul = Math.Pow(def.CostGrowth, Math.Max(0, nextLevel - 1));
        return def.BaseCost * mul;
    }

    public void ApplyPassiveToStat(ref PlayerStatBuildContext ctx, GameConfigSO config)
    {
        foreach (var kv in _skillModel.SkillLevels) // IReadOnlyDictionary<int,int> 필요
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

    public void ApplyActiveToDamage(ref DamageContext dmg, GameConfigSO config)
    {
        // “현재 발동 중인 액티브 스킬”을 어떻게 관리할지(쿨/지속)는
        // SkillManager/Combat에서 관리하고 여기엔 '적용 로직'만 두는 게 좋음.
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
}
