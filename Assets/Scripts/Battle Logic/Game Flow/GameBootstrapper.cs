using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private GameCoordinator _coordinator;
    [SerializeField] private InGameUIComposer _uiComposer;
    [SerializeField] private GameConfigSO _gameConfigSO;

    [Header("Save Load Config")] // 이것도 SO로 빼도 될 듯
    [SerializeField] private float _saveDebounceSeconds = 2.0f;
    [SerializeField] private float _maxSaveIntervalSeconds = 15.0f;


    private GameContext _gameContext;

    // Models
    private StageModel _stageModel;
    private MonsterHpModel _monsterHpModel;
    private RelicModel _relicModel;
    private UpgradeModel _upgradeModel;
    private SkillModel _skillModel;
    private SkillCooldownModel _skillCooldownModel;
    private SkillSlotModel _skillSlotModel; 
    private WalletModel _walletModel;
    private GameStateModel _gameStateModel;

    // Services
    private SaveLoadService _saveLoadService;
    private SavePatchBuilder _savePatchBuilder;

    private StatBuilderService _statBuilderService;

    private StageProgressService _stageProgressService;
    private StageMaxHpService _stageMaxHpService;
    private MonsterHpService _monsterHpService;

    private RelicService _relicService;
    private RelicGachaService _relicGachaService;

    private UpgradeService _upgradeService;
    private SkillService _skillService;

    private CombatService _combatService;
    private WalletService _walletService;

    private RewardService _rewardService;
    private PurchaseService _purchaseService;

    // Managers
    // - Infrastructure Managers
    // - 게임 전반에서 공통적으로 사용되는 인프라/저장/구매 시스템
    private SaveLoadManager _saveLoadManager;
    private ISaveMark _saveMark;
    private PurchaseManager _purchaseManager;
    private WalletManager _walletManager;

    // - Domain Managers
    // - 각 도메인의 상태(Model)와 규칙을 소유하는 핵심 로직 계층
    // - 서로 독립적으로 작동
    private StageManager _stageManager;
    private RelicManager _relicManager;
    private UpgradeManager _upgradeManager;
    private SkillManager _skillManager;
    private RewardManager _rewardManager;
    private StatManager _statManager;

    // - Coordinators
    // - 여러 Domain Manager와 Infrastructure를 조합하여
    // - 하나의 게임 흐름 및 유스케이스를 제공하는 상위 계층
    private CombatCoordinator _combatCoordinator;
    // private BossTimerCoordinator _bossTimerCoordinator;


    private void Awake()
    {
        BuildContext();
        _coordinator.Init(_gameContext);
        StartGameAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private void Start()
    {
        _uiComposer.Init(_gameContext, _gameConfigSO);
        _uiComposer.Compose();
    }


    private async UniTaskVoid StartGameAsync(CancellationToken ct)
    {
        await _gameContext.SaveLoadManager.LoadAllAsync(ct);
        _coordinator.Activate();
        ActivateManagers();
    }

    private void BuildContext()
    {
        ConstructModels();
        ConstructServices();
        ConstructManagers();

        _gameContext = new()
        {
            GameStateModel = _gameStateModel,

            SaveLoadManager = _saveLoadManager,

            StatManager = _statManager,
            StageManager = _stageManager,

            RelicManager = _relicManager,
            UpgradeManager = _upgradeManager,
            SkillManager = _skillManager,

            CombatCoordinator = _combatCoordinator,
            WalletManager = _walletManager,

            RewardManager = _rewardManager,
            PurchaseManager = _purchaseManager
        };

        InitializeManagers();
    }

    private void ConstructModels()
    {
        _stageModel = new();
        _monsterHpModel = new();
        _relicModel = new();
        _upgradeModel = new();
        _skillModel = new();
        _skillCooldownModel = new();
        _skillSlotModel = new();
        _walletModel = new();

        _gameStateModel = new(
            relicModel: _relicModel,
            skillModel: _skillModel,
            skillCooldownModel: _skillCooldownModel,
            skillSlotModel: _skillSlotModel,
            stageModel: _stageModel,
            monsterHpModel: _monsterHpModel,
            upgradeModel: _upgradeModel,
            walletModel: _walletModel
        );
    }

    private void ConstructServices()
    {
        _savePatchBuilder = new();
        _saveLoadService = new();

        _statBuilderService = new();

        _stageProgressService = new(_gameStateModel.StageModel);
        _stageMaxHpService = new(_gameConfigSO.StageConfigSO);
        _monsterHpService = new(_monsterHpModel);

        _relicService = new(_gameStateModel.RelicModel);
        _relicGachaService = new(_gameStateModel.RelicModel, _gameConfigSO);

        _upgradeService = new(_gameStateModel.UpgradeModel);
        _skillService = new(
            _gameStateModel.SkillModel,
            _gameStateModel.SkillCooldownModel,
            _gameStateModel.SkillSlotModel
            );

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
            saveDebounceSeconds: _saveDebounceSeconds,
            maxSaveIntervalSeconds: _maxSaveIntervalSeconds
        );
        _saveMark = _saveLoadManager;

        _purchaseManager = new(_purchaseService);
        _stageManager = new(_stageProgressService, _stageMaxHpService, _monsterHpService);
        _relicManager = new(_relicService, _relicGachaService, _gameConfigSO);
        _upgradeManager = new(_upgradeService, _gameConfigSO);
        _skillManager = new(_skillService, _gameConfigSO);
        _walletManager = new(_walletService);
        _rewardManager = new(_rewardService, _gameConfigSO);
        _combatCoordinator = new(_combatService, _gameConfigSO.SkillConfigSO);

        _statManager = new(
            _statBuilderService,

            _gameConfigSO
        );
    }

    public void InitializeManagers()
    {
        _saveLoadManager.Initialize();
        _purchaseManager.Initialize(_saveMark);
        _stageManager.Initialize(_saveMark);
        _relicManager.Initialize(_saveMark, _purchaseManager);
        _upgradeManager.Initialize(_saveMark, _purchaseManager);
        _skillManager.Initialize(_saveMark, _purchaseManager);
        _walletManager.Initialize(_saveMark);
        _rewardManager.Initialize();
        _combatCoordinator.Initialize(_statManager, _skillManager, _stageManager,
            new IStatModifier[]
            {
                _skillManager
            });
        _statManager.Initialize(new IStatContributor[]
            {
                _upgradeManager,
                _relicManager,
                _skillManager
            });
    }

    public void ActivateManagers()
    {
        _saveLoadManager.Activate();
        _purchaseManager.Activate();
        _stageManager.Activate();
        _relicManager.Activate();
        _upgradeManager.Activate();
        _skillManager.Activate();
        _walletManager.Activate();
        _rewardManager.Activate();
        _combatCoordinator.Activate();
        _statManager.Activate();
    }

    private void OnDestroy() => DeactivateManagers();

    private void DeactivateManagers()
    {
        _statManager.Deactivate();
        _combatCoordinator.Deactivate();
        _rewardManager.Deactivate();
        _walletManager.Deactivate();
        _skillManager.Deactivate();
        _upgradeManager.Deactivate();
        _relicManager.Deactivate();
        _stageManager.Deactivate();
        _purchaseManager.Deactivate();
        _saveLoadManager.Deactivate();
    }
}
