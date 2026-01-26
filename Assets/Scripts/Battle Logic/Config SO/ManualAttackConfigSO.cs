using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ManualAttackConfigSO")]
public class ManualAttackConfigSO : ScriptableObject
{
    public AnimationKey AttackAnimationKey = AnimationKey.Player_Melee_0;
}
