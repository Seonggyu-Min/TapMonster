using System.Collections.Generic;
using UnityEngine;


public class SkillCooldownModel
{
    private readonly Dictionary<int, float> _readyTime = new(); // 사용 가능 시간

    public bool CanUse(int skillId, float now)
        => !_readyTime.TryGetValue(skillId, out float t) || now >= t;

    public float GetRemaining(int skillId, float now)
        => _readyTime.TryGetValue(skillId, out float t) ? Mathf.Max(0f, t - now) : 0f;

    public void StartCooldown(int skillId, float now, float cooldown)
        => _readyTime[skillId] = now + Mathf.Max(0f, cooldown);

    public void Clear() => _readyTime.Clear();
}
