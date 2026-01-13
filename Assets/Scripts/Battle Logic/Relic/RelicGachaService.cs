using System;
using Unity.VisualScripting.FullSerializer;

public class RelicGachaService
{
    private GameConfigSO _gameConfigSO;
    private RelicModel _relicModel;
    private readonly Random _rng = new Random();

    public RelicGachaService(RelicModel relicModel, GameConfigSO gameConfigSO)
    {
        _relicModel = relicModel;
        _gameConfigSO = gameConfigSO;
    }

    public BigNumber GetCurrentRelicGachaCost()
    {
        int count = _relicModel.TotalRelicGachaCount;

        double mul = Math.Pow(_gameConfigSO.GachaConfigSO.RelicGachaCostGrowth, count);
        return _gameConfigSO.GachaConfigSO.BaseRelicGachaCostGold * mul;
    }


    public int RollRelicId(GameConfigSO config)
    {
        var pool = config.GachaConfigSO.RelicPool;
        if (pool == null || pool.Count == 0) return 0;

        int total = 0;
        for (int i = 0; i < pool.Count; i++)
            total += Math.Max(0, pool[i].Weight);

        if (total <= 0) return pool[0].Id;

        int r = _rng.Next(0, total);
        int acc = 0;

        for (int i = 0; i < pool.Count; i++)
        {
            acc += Math.Max(0, pool[i].Weight);
            if (r < acc) return pool[i].Id;
        }

        return pool[pool.Count - 1].Id;
    }
}
