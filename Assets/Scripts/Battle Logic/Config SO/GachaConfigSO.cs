using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GachaConfig")]
public class GachaConfigSO : ScriptableObject
{
    [Header("Cost")]
    [SerializeField] private BigNumber _baseGachaCostGold = new BigNumber(1, 3); // 1e3
    [SerializeField] private float _relicGachaCostGrowth = 1.15f;

    [Header("Relic Pool")]
    [SerializeField] private List<WeightedId> _relicPool = new();

    [Serializable]
    public struct WeightedId
    {
        public int Id;
        public string Name;
        public int Weight;
        public Sprite Icon;
    }

    public BigNumber BaseRelicGachaCostGold => _baseGachaCostGold;
    public float RelicGachaCostGrowth => _relicGachaCostGrowth;
    public IReadOnlyList<WeightedId> RelicPool => _relicPool;
}
