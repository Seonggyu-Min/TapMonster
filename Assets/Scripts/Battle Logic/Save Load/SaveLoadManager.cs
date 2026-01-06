public class SaveLoadManager
{
    private SaveLoadService _saveLoadService;

    private StageManager _stageManager;
    private RelicManager _relicManager;
    private UpgradeManager _upgradeManager;
    private SkillManager _skillManager;
    private WalletManager _walletManager;

    public SaveLoadManager(
        SaveLoadService saveLoadService,
        StageManager stageManager,
        RelicManager relicManager,
        UpgradeManager upgradeManager,
        SkillManager skillManager,
        WalletManager walletManager
        )
    {
        _saveLoadService = saveLoadService;

        _stageManager = stageManager;
        _relicManager = relicManager;
        _upgradeManager = upgradeManager;
        _skillManager = skillManager;
        _walletManager = walletManager;
    }


    public void Save()
    {

    }

    public void LoadAll()
    {
        _saveLoadService.LoadAll(
            stageManager: _stageManager,
            relicManager: _relicManager,
            upgradeManager: _upgradeManager,
            skillManager: _skillManager,
            walletManager: _walletManager
            );
    }
}
