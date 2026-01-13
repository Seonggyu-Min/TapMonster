using UnityEngine;
using System.Collections.Generic;

public class SkillModel : ILevelModel
{
    private readonly Dictionary<int, int> _skillLevels = new();
    public IReadOnlyDictionary<int, int> SkillLevels => _skillLevels;


    public int GetLevel(int upgradeId) => _skillLevels.TryGetValue(upgradeId, out int level) ? level : 0;
    public void SetLevel(int upgradeId, int level) => _skillLevels[upgradeId] = Mathf.Max(0, level);
    public void AddLevel(int upgradeId, int delta) => SetLevel(upgradeId, GetLevel(upgradeId) + delta);
    public void Clear() => _skillLevels.Clear();
}
