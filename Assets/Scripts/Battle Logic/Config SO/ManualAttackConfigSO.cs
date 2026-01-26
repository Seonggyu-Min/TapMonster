using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ManualAttackConfig")]
public class ManualAttackConfigSO : ScriptableObject
{
    public AnimationKey AttackAnimationKey = AnimationKey.Player_Melee_0;
}
