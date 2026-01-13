using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/StageConfig")]
public class StageConfigSO : ScriptableObject
{
    [Header("Stage Curve")]
    [SerializeField] private int _startStage = 1;

    [Tooltip("Stage HP")]
    [SerializeField] private BigNumber _baseBossHp = new BigNumber(1, 3);
    [SerializeField] private double _bossHpGrowth = 1.15;

    // TODO: 오버라이드 대신 n 배수 마다 자동 보스처리
    [Header("Stage Overrides")]
    [SerializeField] private List<StageOverride> _overrides = new();

    private Dictionary<int, StageOverride> _overrideCache;

    [Serializable]
    public sealed class StageOverride
    {
        public int Stage;
        public BigNumber BossHp;
        public int BossPatternId; // BT 패턴 Id
    }

    public BigNumber GetBossHp(int stage)
    {
        if (stage < _startStage) stage = _startStage;

        _overrideCache ??= BuildOverrideCache();
        if (_overrideCache.TryGetValue(stage, out var ov))
        {
            return ov.BossHp;
        }

        int delta = stage - _startStage;
        return BigNumber.Pow(_baseBossHp, 1) * Math.Pow(_bossHpGrowth, delta);
    }

    private Dictionary<int, StageOverride> BuildOverrideCache()
    {
        var dict = new Dictionary<int, StageOverride>(_overrides.Count);
        foreach (var ov in _overrides)
        {
            dict[ov.Stage] = ov;
        }
        return dict;
    }
}
