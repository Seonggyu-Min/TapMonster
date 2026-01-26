using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AutoAttackConfig")]
public class AutoAttackConfigSO : ScriptableObject
{
    [Header("Visual")]
    public float FlashDurationSeconds = 0.05f;
}
