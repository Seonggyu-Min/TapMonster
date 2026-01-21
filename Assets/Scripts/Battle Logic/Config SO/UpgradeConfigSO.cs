using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/UpgradeConfig")]
public class UpgradeConfigSO : ScriptableObject
{
    [SerializeField] private List<UpgradeDef> _defs = new();

    private Dictionary<int, UpgradeDef> _cache;

    [Serializable]
    public class UpgradeDef : IHasId
    {
        [field: SerializeField] public int Id { get; private set; }

        [Header("Info")]
        public string Name;
        public int MaxLevel = 100;

        [Header("Cost Curve")]
        public BigNumber BaseCost = new BigNumber(1, 2);  // 1e2
        public double CostGrowth = 1.15;                  // base * growth^(level)

        [Header("Effect")]
        public UpgradeStatType StatType = UpgradeStatType.ManualAdditiveDamagePercent;
        public double ValuePerLevel = 1.0;                // 타입에 따라 의미 달라짐

        [Header("Icon")]
        public Sprite Icon;
    }

    public bool TryGet(int id, out UpgradeDef def)
    {
        _cache ??= BuildCache();
        return _cache.TryGetValue(id, out def);
    }

    public IReadOnlyList<UpgradeDef> UpgradeDefs => _defs;

    private Dictionary<int, UpgradeDef> BuildCache()
    {
        var dict = new Dictionary<int, UpgradeDef>(_defs.Count);
        foreach (var d in _defs) dict[d.Id] = d;
        return dict;
    }
}

public enum UpgradeStatType
{
    ManualAdditiveDamagePercent,
    AutoAdditiveDamagePercent,
    ManualDamageMultiplier,
    AutoDamageMultiplier,
    FinalAllDamageMultiplier,
    ManualCriticalChance,
    AutoCriticalChance,
}