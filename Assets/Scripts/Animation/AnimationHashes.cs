using System.Collections.Generic;
using UnityEngine;

public enum AnimationKey
{
    Player_Idle_0 = 0,
    Player_Idle_1,
    Player_Kick_0,
    Player_Melee_0,
    Player_Melee_1,
    Player_Pummel_0,
    Player_Shield_Block_0,
    Player_Special_0,
    Player_Special_1,
    Player_Turn_0,
    Player_Turn_1,
    Player_Turn_2,
    Player_Unsheath_0,
    Player_Cast_Spell_0,
}

public static class AnimationHashes
{
    private static readonly Dictionary<AnimationKey, int> _map = BuildMap();

    private static readonly LogCategory CurrentCategory = LogCategory.Animation;


    private static Dictionary<AnimationKey, int> BuildMap()
    {
        Dictionary<AnimationKey, int> dict = new();

        foreach (AnimationKey key in System.Enum.GetValues(typeof(AnimationKey)))
        {
            string triggerName = key.ToString();
            dict[key] = Animator.StringToHash(triggerName);
        }

        return dict;
    }

    public static int Get(AnimationKey key)
    {
        if (_map.TryGetValue(key, out int hash))
        {
            return hash;
        }

        DebugLogger.PrintLog(
            typeof(AnimationHashes),
            $"{key} 애니메이션 해시가 존재하지 않습니다.",
            CurrentCategory,
            LogType.Warning
        );

        return 0;
    }

    // 디버그용
    public static string GetTriggerName(AnimationKey key) => key.ToString();
}
