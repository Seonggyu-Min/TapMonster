using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/CombatConfig")]
public class CombatConfigSO : ScriptableObject
{
    [Header("Base Damage Multipliers")]
    [SerializeField] private double _tapMultiplier = 1.0;
    [SerializeField] private double _autoMultiplier = 0.3;
    [SerializeField] private double _skillMultiplier = 2.0;

    [Header("Crit")]
    [Range(0, 1f)]
    [SerializeField] private float _baseCritChance = 0.05f;
    [SerializeField] private double _baseCritMultiplier = 2.0;

    [Header("Timing")]
    [SerializeField] private float _autoAttackInterval = 0.2f; // 0.2f = 초당 5회

    public double TapMultiplier => _tapMultiplier;
    public double AutoMultiplier => _autoMultiplier;
    public double SkillMultiplier => _skillMultiplier;

    public float BaseCritChance => _baseCritChance;
    public double BaseCritMultiplier => _baseCritMultiplier;

    public float AutoAttackInterval => _autoAttackInterval;
}
