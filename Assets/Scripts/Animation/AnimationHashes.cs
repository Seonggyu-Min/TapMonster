using System.Collections.Generic;
using UnityEngine;

public enum AnimationSubject
{
    None = 0,
    Player = 1,
    Monster = 2,
    Boss = 3
}

public enum AnimationKey
{
    Idle_0 = 0,
    Idle_1,
    Kick_0,
    Melee_0,
    Melee_1,
    Pummel_0,
    Shield_Block_0,
    Special_0,
    Special_1,
    Turn_0,
    Turn_1,
    Turn_2,
    Unsheath_0,
    Cast_Spell_0,
}


public static class AnimationHashes
{
    public static readonly int Player_Idle_0 = Animator.StringToHash("Player_Idle_0");
    public static readonly int Player_Idle_1 = Animator.StringToHash("Player_Idle_1");
    public static readonly int Player_Kick_0 = Animator.StringToHash("Player_Kick_0");
    public static readonly int Player_Melee_0 = Animator.StringToHash("Player_Melee_0");
    public static readonly int Player_Melee_1 = Animator.StringToHash("Player_Melee_1");
    public static readonly int Player_Pummel_0 = Animator.StringToHash("Player_Pummel_0");
    public static readonly int Player_Shield_Block_0 = Animator.StringToHash("Player_Shield_Block_0");
    public static readonly int Player_Special_0 = Animator.StringToHash("Player_Special_0");
    public static readonly int Player_Special_1 = Animator.StringToHash("Player_Special_1");
    public static readonly int Player_Turn_0 = Animator.StringToHash("Player_Turn_0");
    public static readonly int Player_Turn_1 = Animator.StringToHash("Player_Turn_1");
    public static readonly int Player_Turn_2 = Animator.StringToHash("Player_Turn_2");
    public static readonly int Player_Unsheath_0 = Animator.StringToHash("Player_Unsheath_0");
    public static readonly int Player_Cast_Spell_0 = Animator.StringToHash("Player_Cast_Spell_0");


    private static LogCategory CurrentCategory = LogCategory.Animation;


    private static readonly Dictionary<(AnimationSubject, AnimationKey), int> _map =
        new()
        {
            // Player
            {(AnimationSubject.Player, AnimationKey.Idle_0), Player_Idle_0},
            {(AnimationSubject.Player, AnimationKey.Idle_1), Player_Idle_1},
            {(AnimationSubject.Player, AnimationKey.Kick_0), Player_Kick_0},
            {(AnimationSubject.Player, AnimationKey.Melee_0), Player_Melee_0},
            {(AnimationSubject.Player, AnimationKey.Melee_1), Player_Melee_1},
            {(AnimationSubject.Player, AnimationKey.Pummel_0), Player_Pummel_0},
            {(AnimationSubject.Player, AnimationKey.Shield_Block_0), Player_Shield_Block_0},
            {(AnimationSubject.Player, AnimationKey.Special_0), Player_Special_0},
            {(AnimationSubject.Player, AnimationKey.Special_1), Player_Special_1},
            {(AnimationSubject.Player, AnimationKey.Turn_0), Player_Turn_0},
            {(AnimationSubject.Player, AnimationKey.Turn_1), Player_Turn_1},
            {(AnimationSubject.Player, AnimationKey.Turn_2), Player_Turn_2},
            {(AnimationSubject.Player, AnimationKey.Unsheath_0), Player_Unsheath_0},
            {(AnimationSubject.Player, AnimationKey.Cast_Spell_0), Player_Cast_Spell_0},

            // Monster

            // Boss
        };


    public static int Get(AnimationSubject subject, AnimationKey key)
    {
        if (_map.TryGetValue((subject, key), out int hash))
            return hash;

        DebugLogger.PrintLog(typeof(AnimationHashes), $"{subject} / {key} 해당 애니메이션 매핑이 존재하지 않습니다", CurrentCategory, LogType.Warning);
        return 0;
    }
}
