using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GameConfig")]
public class GameConfigSO : ScriptableObject
{
    [SerializeField] private StageConfigSO _stageConfigSO;
    [SerializeField] private RewardConfigSO _rewardConfigSO;
    [SerializeField] private CombatConfigSO _combatConfigSO;
    [SerializeField] private GachaConfigSO _gachaConfigSO;


    public StageConfigSO StageConfigSO => _stageConfigSO;
    public RewardConfigSO RewardConfigSO => _rewardConfigSO;
    public CombatConfigSO CombatConfigSO => _combatConfigSO;
    public GachaConfigSO GachaConfigSO => _gachaConfigSO;
}
