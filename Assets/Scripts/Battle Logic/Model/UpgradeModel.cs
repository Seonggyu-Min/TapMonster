using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeModel : ILevelModel
{
    private readonly Dictionary<int, int> _upgradeLevels = new();
    public IReadOnlyDictionary<int, int> UpgradeLevels => _upgradeLevels;
    public event Action<int, int> OnUpgradeLevelChanged;


    public int GetLevel(int upgradeId) => _upgradeLevels.TryGetValue(upgradeId, out int level) ? level : 1;
    public void SetLevel(int upgradeId, int level)
    {
        _upgradeLevels[upgradeId] = Mathf.Max(1, level);
        OnUpgradeLevelChanged?.Invoke(upgradeId, level);
    }
    public void AddLevel(int upgradeId, int delta) => SetLevel(upgradeId, GetLevel(upgradeId) + delta);
    public void Clear() => _upgradeLevels.Clear();

}
