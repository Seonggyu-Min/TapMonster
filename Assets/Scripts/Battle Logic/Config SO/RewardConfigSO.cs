using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/RewardConfig")]
public class RewardConfigSO : ScriptableObject
{
    [Header("Gold Reward Curve")]
    [SerializeField] private BigNumber _baseGold = new BigNumber(5, 1); // 50
    [SerializeField] private double _goldGrowth = 1.12;

    public BigNumber GetStageClearGold(int stage)
    {
        if (stage < 1) stage = 1;
        int delta = stage - 1;
        return _baseGold * Math.Pow(_goldGrowth, delta);
    }
}
