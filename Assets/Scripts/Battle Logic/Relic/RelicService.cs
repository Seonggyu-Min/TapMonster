public class RelicService
{
    private RelicModel _relicModel;


    public RelicService(RelicModel relicModel)
    {
        _relicModel = relicModel;
    }


    public int GetLevel(int relicId) => _relicModel.GetLevel(relicId);
    
    // TODO: 풀 레벨 업 이면 다른 처리
    public void AddLevel(int relicId, int delta) => _relicModel.AddLevel(relicId, delta);

    public void ApplyToStat(ref PlayerStatBuildContext ctx, GameConfigSO config)
    {
        foreach (var kv in _relicModel.RelicLevels) // IReadOnlyDictionary<int,int> 필요
        {
            int id = kv.Key;
            int level = kv.Value;
            if (level <= 0) continue;

            if (!config.RelicConfigSO.TryGet(id, out var def))
                continue;

            double v = def.ValuePerLevel * level;

            switch (def.StatType)
            {
                case RelicStatType.ManualAdditiveDamagePercent:
                    ctx.ManualAdditiveDamagePercent += (float)v;
                    break;

                case RelicStatType.AutoAdditiveDamagePercent:
                    ctx.AutoAdditiveDamagePercent += (float)v;
                    break;

                case RelicStatType.ManualDamageMultiplier:
                    ctx.ManualDamageMultiplier *= (float)v;
                    break;

                case RelicStatType.AutoDamageMultiplier:
                    ctx.AutoDamageMultiplier *= (float)v;
                    break;

                case RelicStatType.FinalAllDamageMultiplier:
                    ctx.FinalAllDamageMultiplier *= (float)v;
                    break;

                case RelicStatType.ManualCriticalChance:
                    ctx.ManualCriticalChance += (float)v;
                    break;

                case RelicStatType.AutoCriticalChance:
                    ctx.AutoCriticalChance += (float)v;
                    break;
            }
        }
    }


    public void IncrementRelicGachaCount()
    {
        _relicModel.IncrementlRelicGachaCount();
    }
}
