using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GameConfig")]
public class GameConfigSO : ScriptableObject
{
    [SerializeField] private StageConfigSO _stageConfigSO;
    [SerializeField] private RewardConfigSO _rewardConfigSO;
    [SerializeField] private CombatConfigSO _combatConfigSO;
    [SerializeField] private GachaConfigSO _gachaConfigSO;
    [SerializeField] private RelicConfigSO _relicConfigSO;
    [SerializeField] private UpgradeConfigSO _upgradeConfigSO;
    [SerializeField] private SkillConfigSO _skillConfigSO;
    [SerializeField] private ManualAttackConfigSO _manualAttackConfigSO;

    public StageConfigSO StageConfigSO => _stageConfigSO;
    public RewardConfigSO RewardConfigSO => _rewardConfigSO;
    public CombatConfigSO CombatConfigSO => _combatConfigSO;
    public GachaConfigSO GachaConfigSO => _gachaConfigSO;
    public RelicConfigSO RelicConfigSO => _relicConfigSO;
    public UpgradeConfigSO UpgradeConfigSO => _upgradeConfigSO;
    public SkillConfigSO SkillConfigSO => _skillConfigSO;
    public ManualAttackConfigSO ManualAttackConfigSO => _manualAttackConfigSO;
}
