using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private GameCoordinator _coordinator;
    [SerializeField] private GameConfigSO _gameConfigSO;

    private GameContext _ctx;

    // Models
    private StageModel _stageModel;
    private RelicModel _relicModel;
    private UpgradeModel _upgradeModel;
    private SkillModel _skillModel;
    private WalletModel _walletModel;
    private GameStateModel _gameStateModel;

    // Services
    private SaveLoadService _saveLoadService;
    private SavePatchBuilder _savePatchBuilder;

    private StatBuilderService _statBuilderService;

    private StageService _stageService;

    private RelicService _relicService;
    private RelicGachaService _relicGachaService;

    private UpgradeService _upgradeService;
    private SkillService _skillService;

    private CombatService _combatService;
    private WalletService _walletService;

    private RewardService _rewardService;
    private PurchaseService _purchaseService;

    // Managers
    private SaveLoadManager _saveLoadManager;

    private StageManager _stageManager;
    private RelicManager _relicManager;
    private UpgradeManager _upgradeManager;
    private SkillManager _skillManager;

    private CombatManager _combatManager;
    private WalletManager _walletManager;

    private RewardManager _rewardManager;
    private PurchaseManager _purchaseManager;

    private StatManager _statManager;



    private void Awake()
    {
        _ctx = BuildContext();
        _coordinator.Init(_ctx);

        StartGameAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }


    private async UniTaskVoid StartGameAsync(CancellationToken ct)
    {
        await _ctx.SaveLoadManager.LoadAllAsync(ct);
        //_ctx.StatManager.RebuildSnapshot();
    }

    private GameContext BuildContext()
    {
        ConstructModels();
        ConstructServices();
        ConstructManagers();

        return new GameContext
        {
            GameStateModel = _gameStateModel,

            SaveLoadManager = _saveLoadManager,

            StatManager = _statManager,
            StageManager = _stageManager,

            RelicManager = _relicManager,
            UpgradeManager = _upgradeManager,
            SkillManager = _skillManager,

            CombatManager = _combatManager,
            WalletManager = _walletManager,

            RewardManager = _rewardManager,
            PurchaseManager = _purchaseManager
        };
    }

    private void ConstructModels()
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

    private void ConstructServices()
    {
        _saveLoadService = new();
        _savePatchBuilder = new();

        _statBuilderService = new();

        _stageService = new(_gameStateModel.StageModel);

        _relicService = new(_gameStateModel.RelicModel);
        _relicGachaService = new(_gameStateModel.RelicModel, _gameConfigSO);

        _upgradeService = new(_gameStateModel.UpgradeModel);
        _skillService = new(_gameStateModel.SkillModel);

        _combatService = new();
        _walletService = new(_gameStateModel.WalletModel);

        _rewardService = new();

        _purchaseService = new(_walletService);
    }

    private void ConstructManagers()
    {
        string uid = FirebaseManager.Instance.Auth.CurrentUser.UserId;

        _saveLoadManager = new(
            uid: uid,
            gameState: _gameStateModel,
            service: _saveLoadService,
            savePatchBuilder: _savePatchBuilder,
            saveDebounceSeconds: 2.0f,
            maxSaveIntervalSeconds: 15.0f
        );

        ISaveMark saveMark = _saveLoadManager;

        _purchaseManager = new(_purchaseService, saveMark);
        _stageManager = new(_stageService, _gameConfigSO, saveMark);
        _relicManager = new(_relicService, _relicGachaService, _gameConfigSO, _purchaseManager, saveMark);
        _upgradeManager = new(_upgradeService, _gameConfigSO, _purchaseManager, saveMark);
        _skillManager = new(_skillService, _gameConfigSO, _purchaseManager, saveMark);
        _combatManager = new(_combatService);
        _walletManager = new(_walletService, saveMark);
        _rewardManager = new(_rewardService, _gameConfigSO);

        _statManager = new(
            _statBuilderService,
            new IStatContributor[]
            {
                _upgradeManager,
                _relicManager,
                _skillManager
            },
            _gameConfigSO
        );
    }
}
