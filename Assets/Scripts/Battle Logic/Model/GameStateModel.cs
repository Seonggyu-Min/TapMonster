public class GameStateModel
{
    private RelicModel _relicModel;
    private SkillModel _skillModel;
    private StageModel _stageModel;
    private UpgradeModel _upgradeModel;
    private WalletModel _walletModel;


    public GameStateModel(
        RelicModel relicModel,
        SkillModel skillModel,
        StageModel stageModel,
        UpgradeModel upgradeModel,
        WalletModel walletModel
        )
    {
        _relicModel = relicModel;
        _skillModel = skillModel;
        _stageModel = stageModel;
        _upgradeModel = upgradeModel;
        _walletModel = walletModel;
    }


    public RelicModel RelicModel => _relicModel;
    public SkillModel SkillModel => _skillModel;
    public StageModel StageModel => _stageModel;
    public UpgradeModel UpgradeModel => _upgradeModel;
    public WalletModel WalletModel => _walletModel;
}
