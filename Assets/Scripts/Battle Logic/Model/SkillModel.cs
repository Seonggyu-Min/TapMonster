using UnityEngine;
using System.Collections.Generic;
using System;

public class SkillModel : ILevelModel
{
    private readonly Dictionary<int, int> _skillLevels = new();
    public IReadOnlyDictionary<int, int> SkillLevels => _skillLevels;
    public event Action<int, int> OnSkillLevelChanged;


    public int GetLevel(int upgradeId) => _skillLevels.TryGetValue(upgradeId, out int level) ? level : 0;
    public void SetLevel(int upgradeId, int level)
    {
        _skillLevels[upgradeId] = Mathf.Max(0, level);
        OnSkillLevelChanged?.Invoke(upgradeId, level);
    }
    public void AddLevel(int upgradeId, int delta) => SetLevel(upgradeId, GetLevel(upgradeId) + delta);
    public void Clear() => _skillLevels.Clear();

    // TODO: DTO 로드 시 한 프레임에 다수의 OnSkillLevelChanged 호출 최적화
    // 그래서 SetBulkLevels 같은 메서드 추가해서 한번에 여러 레벨 설정 가능하게 하고
    // 한 번만 OnSkillLevelChanged 호출하게 만들기
    // 안그러면 Presenter에서 매번 갱신하게 돼서 비효율적임
}
