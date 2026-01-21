using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/SkillConfig")]
public class SkillConfigSO : ScriptableObject
{
    [SerializeField] private List<SkillDef> _defs = new();
    private Dictionary<int, SkillDef> _cache;

    [Serializable]
    public sealed class SkillDef : IHasId
    {
        [field: SerializeField] public int Id { get; private set; }

        [Header("Info")]
        public string Name;
        public int MaxLevel = 100;

        [Header("Kind")]
        public SkillKind Kind = SkillKind.Passive;

        [Header("Cost Curve (Level Up)")]
        public BigNumber BaseCost = new BigNumber(5, 2);
        public double CostGrowth = 1.18;

        [Header("Cooldown (Active Only)")]
        public float CooldownSeconds = 5f;

        [Header("Passive Effect")]
        public SkillPassiveType PassiveType = SkillPassiveType.None;
        public double PassiveValuePerLevel = 0.02;

        [Header("Active Effect (DamageContext)")]
        public SkillActiveType ActiveType = SkillActiveType.None;
        public double ActiveMulPerLevel = 0.10;

        [Header("Icon")]
        public Sprite Icon;
    }

    public bool TryGet(int id, out SkillDef def)
    {
        _cache ??= BuildCache();
        return _cache.TryGetValue(id, out def);
    }

    public IReadOnlyList<SkillDef> SkillDefs => _defs;

    private Dictionary<int, SkillDef> BuildCache()
    {
        var dict = new Dictionary<int, SkillDef>(_defs.Count);
        foreach (var d in _defs) dict[d.Id] = d;
        return dict;
    }
}


public enum SkillKind { Passive, Active }

public enum SkillPassiveType
{
    None,
    ManualAdditiveDamagePercent,
    AutoAdditiveDamagePercent,
    ManualDamageMultiplier,
    AutoDamageMultiplier,
    FinalAllDamageMultiplier,
    ManualCriticalChance,
    AutoCriticalChance
}

public enum SkillActiveType
{
    None,
    ManualDamageMul,  // Manual일 때만 곱
    AutoDamageMul,    // Auto일 때만 곱
    //NormalDamageMul,  // Normal 대상일 때만 곱
    //BossDamageMul,    // Boss 대상일 때만 곱
}