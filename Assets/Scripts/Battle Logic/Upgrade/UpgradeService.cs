using System;

public class UpgradeService
{
    private UpgradeModel _upgradeModel;

    public event Action<int, int> OnUpgradeLevelChanged
    {
        add => _upgradeModel.OnUpgradeLevelChanged += value;
        remove => _upgradeModel.OnUpgradeLevelChanged -= value;
    }

    public UpgradeService(UpgradeModel upgradeModel)
    {
        _upgradeModel = upgradeModel;
    }

    public int GetLevel(int id) => _upgradeModel.GetLevel(id);
    public void AddLevel(int id, int delta) => _upgradeModel.AddLevel(id, delta);

    public BigNumber GetLevelUpCost(GameConfigSO config, int upgradeId, int nextLevel)
    {
        if (!config.UpgradeConfigSO.TryGet(upgradeId, out var def))
            return BigNumber.One;

        double mul = Math.Pow(def.CostGrowth, Math.Max(0, nextLevel - 1));
        return def.BaseCost * mul;
    }

    public void ApplyToStat(ref PlayerStatBuildContext ctx, GameConfigSO config)
    {
        // 업그레이드 전체를 ctx에 누적
        foreach (var kv in _upgradeModel.UpgradeLevels) // IReadOnlyDictionary<int,int> 필요
        {
            int id = kv.Key;
            int level = kv.Value;
            if (level <= 0) continue;

            if (!config.UpgradeConfigSO.TryGet(id, out var def))
                continue;

            ApplyOne(ref ctx, def, level);
        }
    }

    private static void ApplyOne(ref PlayerStatBuildContext ctx, UpgradeConfigSO.UpgradeDef def, int level)
    {
        double v = def.ValuePerLevel * level;

        switch (def.StatType)
        {
            case UpgradeStatType.ManualAdditiveDamagePercent:
                ctx.ManualAdditiveDamagePercent += (float)v;
                break;

            case UpgradeStatType.AutoAdditiveDamagePercent:
                ctx.AutoAdditiveDamagePercent += (float)v;
                break;

            case UpgradeStatType.ManualDamageMultiplier:
                ctx.ManualDamageMultiplier *= (float)v;
                break;

            case UpgradeStatType.AutoDamageMultiplier:
                ctx.AutoDamageMultiplier *= (float)v;
                break;

            case UpgradeStatType.FinalAllDamageMultiplier:
                ctx.FinalAllDamageMultiplier *= (float)v;
                break;

            case UpgradeStatType.ManualCriticalChance:
                ctx.ManualCriticalChance += (float)v;
                break;

            case UpgradeStatType.AutoCriticalChance:
                ctx.AutoCriticalChance += (float)v;
                break;
        }
    }
}
