using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/RelicConfig")]
public class RelicConfigSO : ScriptableObject
{
    [SerializeField] private List<RelicDef> _defs = new();
    private Dictionary<int, RelicDef> _cache;

    [Serializable]
    public sealed class RelicDef : IHasId
    {
        [field: SerializeField] public int Id { get; private set; }

        [Header("Passive Effect")]
        public RelicStatType StatType = RelicStatType.ManualAdditiveDamagePercent;
        public double ValuePerLevel = 0.05; // 예: 레벨당 5% 증폭
    }

    public bool TryGet(int id, out RelicDef def)
    {
        _cache ??= BuildCache();
        return _cache.TryGetValue(id, out def);
    }

    public IReadOnlyList<RelicDef> All => _defs;

    private Dictionary<int, RelicDef> BuildCache()
    {
        var dict = new Dictionary<int, RelicDef>(_defs.Count);
        foreach (var d in _defs) dict[d.Id] = d;
        return dict;
    }
}

public enum RelicStatType
{
    ManualAdditiveDamagePercent,
    AutoAdditiveDamagePercent,
    ManualDamageMultiplier,
    AutoDamageMultiplier,
    FinalAllDamageMultiplier,
    ManualCriticalChance,
    AutoCriticalChance
}