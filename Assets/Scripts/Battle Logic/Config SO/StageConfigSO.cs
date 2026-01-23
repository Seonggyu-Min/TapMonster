using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/StageConfig")]
public class StageConfigSO : ScriptableObject
{
    [Header("Stage Curve")]
    [SerializeField] private int _startStage = 1;

    [Header("Normal Monster HP")]
    [SerializeField] private BigNumber _baseNormalHp = new(1, 1);
    [SerializeField] private double _normalHpGrowth = 1.12;

    [Header("Boss Monster HP")]
    [SerializeField] private BigNumber _baseBossHp = new(1, 3);
    [SerializeField] private double _bossHpGrowth = 1.15;

    [Header("Boss Stage Settings")]
    [SerializeField] private bool _useIntervalBoss = true;
    [SerializeField] private int _bossInterval = 50;   // 50 스테이지마다 보스

    [Header("Stage Overrides for Boss")]
    [SerializeField] private List<StageOverride> _overrides = new();

    private Dictionary<int, StageOverride> _overrideCache;


    [Serializable]
    public sealed class StageOverride
    {
        public int Stage;
        public BigNumber BossHp;
        public int BossPatternId; // BT 패턴 Id
    }

    public bool IsBossStage(int stage)
    {
        if (stage < _startStage) return false;

        _overrideCache ??= BuildOverrideCache();
        // 1. Override 되면 무조건 Boss
        if (_overrideCache.ContainsKey(stage)) return true;
        // 2. N배수 규칙
        if (_useIntervalBoss && stage % _bossInterval == 0) return true;

        return false;
    }

    public BigNumber GetMaxHp(int stage)
    {
        return IsBossStage(stage)
            ? GetBossHp(stage)
            : GetNormalHp(stage);
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

    private BigNumber GetBossHp(int stage)
    {
        if (stage < _startStage) stage = _startStage;

        _overrideCache ??= BuildOverrideCache();

        // 1) Override 있으면 그대로 사용
        if (_overrideCache.TryGetValue(stage, out var ov))
            return ov.BossHp;

        // 2) 자동 보스 HP 커브
        int delta = stage - _startStage;
        return _baseBossHp * Math.Pow(_bossHpGrowth, delta);
    }

    private BigNumber GetNormalHp(int stage)
    {
        if (stage < _startStage) stage = _startStage;

        int delta = stage - _startStage;
        return _baseNormalHp * Math.Pow(_normalHpGrowth, delta);
    }
}
