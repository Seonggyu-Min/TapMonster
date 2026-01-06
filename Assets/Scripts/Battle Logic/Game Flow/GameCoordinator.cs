using UnityEngine;

public class GameCoordinator : MonoBehaviour
{
    #region Fields and Properties

    [SerializeField] private InputManager _inputManager;
    [SerializeField] private GameConfigSO _gameConfigSO;


    // Stat
    private StatManager _statManager;
    private StatBuilderService _statBuilderService;

    // Stage
    private StageManager _stageManager;
    private StageService _stageService;
    private StageModel _stageModel;

    // Relic
    private RelicManager _relicManager;
    private RelicService _relicService;
    private RelicGachaService _relicGachaService;
    private RelicModel _relicModel;

    // Upgrade
    private UpgradeManager _upgradeManager;
    private UpgradeService _upgradeService;
    private UpgradeModel _upgradeModel;

    // Skill
    private SkillManager _skillManager;
    private SkillService _skillService;
    private SkillModel _skillModel;

    // Combat
    private CombatManager _combatManager;
    private CombatService _combatService;

    // Wallet
    private WalletManager _walletManager;
    private WalletService _walletService;
    private WalletModel _walletModel;

    // Reward
    private RewardManager _rewardManager;
    private RewardService _rewardService;

    // Shop
    private ShopManager _shopManager;
    private ShopService _shopService;

    // Model Set
    private GameStateModel _gameStateModel;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        ConstructModel();
        ConstructAndInjectServices();
        ConstructAndInjectManagers();
    }

    private void OnEnable()
    {
        HandleSubscribe(true);
    }

    private void OnDisable()
    {
        HandleSubscribe(false);
    }

    #endregion


    #region Private Methods

    private void ConstructModel()
    {
        _stageModel = new();
        _relicModel = new();
        _upgradeModel = new();
        _skillModel = new();
        _walletModel = new();

        _gameStateModel = new(
            relicModel: _relicModel,
            skillModel: _skillModel,
            stageModel: _stageModel,
            upgradeModel: _upgradeModel,
            walletModel: _walletModel
            );
    }

    private void ConstructAndInjectServices()
    {
        _statBuilderService = new();
        _stageService = new(_gameStateModel.StageModel);
        _relicService = new(_gameStateModel.RelicModel);
        _relicGachaService = new(_gameStateModel.RelicModel);
        _upgradeService = new(_gameStateModel.UpgradeModel);
        _skillService = new(_gameStateModel.SkillModel);
        _combatService = new();
        _walletService = new(_gameStateModel.WalletModel);
        _rewardService = new();
        _shopService = new(_relicModel, _walletModel);
    }

    private void ConstructAndInjectManagers()
    {
        _stageManager = new(_stageService, _gameConfigSO);
        _relicManager = new(_relicService, _relicGachaService, _gameConfigSO);
        _upgradeManager = new(_upgradeService, _gameConfigSO);
        _walletManager = new();
        _skillManager = new(_skillService, _gameConfigSO);
        _combatManager = new(_combatService);
        _rewardManager = new(_rewardService, _gameConfigSO);
        _shopManager = new(_shopService);

        _statManager = new(
            _statBuilderService,
            new IStatContributor[]
            {
                _upgradeManager,
                _relicManager,
                _skillManager
            },
        _gameConfigSO);
    }


    private void HandleSubscribe(bool willSubscribe)
    {
        if (willSubscribe)
        {
            _inputManager.OnManualAttack += HandleManualAttack;

        }


    }


    private void HandleManualAttack()
    {
        this.PrintLog("클릭");
    }

    #endregion
}
