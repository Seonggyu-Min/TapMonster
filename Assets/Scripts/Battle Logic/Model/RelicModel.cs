using System.Collections.Generic;
using UnityEngine;

public class RelicModel : ILevelModel
{
    private readonly Dictionary<int, int> _relicLevels = new();
    public IReadOnlyDictionary<int, int> RelicLevels => _relicLevels;


    public int GetLevel(int upgradeId) => _relicLevels.TryGetValue(upgradeId, out int level) ? level : 0;
    public void SetLevel(int upgradeId, int level) => _relicLevels[upgradeId] = Mathf.Max(0, level);
    public void AddLevel(int upgradeId, int delta) => SetLevel(upgradeId, GetLevel(upgradeId) + delta);
    public void Clear() => _relicLevels.Clear();


    // 가챠 관련
    private int _totalRelicGachaCount = 0;
    public int TotalRelicGachaCount => _totalRelicGachaCount;
    public void SetTotalRelicGachaCount(int count) => _totalRelicGachaCount = Mathf.Max(0, count);
    public void IncrementlRelicGachaCount() => _totalRelicGachaCount++;
}
