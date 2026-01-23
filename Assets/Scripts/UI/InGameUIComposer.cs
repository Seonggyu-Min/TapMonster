using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIComposer : MonoBehaviour
{
    #region Fields

    [Header("Upgrade Panel Refs")]
    [SerializeField] private UpgradePanelView _upgradePanelView;
    
    [Header("Skill Panel Refs")]
    [SerializeField] private SkillPanelView _skillPanelView;

    [Header("Skill Inventory Panel Refs")]
    [SerializeField] private SkillUIOrchestrator _skillUIOrchestrator;

    [Header("Skill Slot Panel Refs")]
    [SerializeField] private SkillLoadoutItemView[] _skillLoadoutItemViews;
    
    [Header("Manual Input Refs")]
    [SerializeField] private Button _manualAttackButton;

    [Header("Monster HP UI Refs")]
    [SerializeField] private MonsterHpView _monsterHpView;

    [Header("Damage Text UI Refs")]
    [SerializeField] private Transform _poolParent;              // 풀 보관용
    [SerializeField] private RectTransform _spawnPoint;              // 스폰 포인트
    [SerializeField] private DamageTextView _damageTextViewPrefab;

    [Header("Stage Index UI Refs")]
    [SerializeField] private StageIndexView _stageIndexView;


    private List<IDisposable> _disposables = new();
    private GameContext _gameContext;
    private GameConfigSO _gameConfigSO;

    // Spawners
    private PoolHub _poolHub;


    // Presenters
    private UpgradePanelPresenter _upgradePanelPresenter;
    private SkillPanelPresenter _skillPanelPresenter;
    private SkillLoadoutPresenter _skillLoadoutPresenter;
    private ManualInputPresenter _manualInputPresenter;
    private MonsterHpUIPresenter _monsterHpUIPresenter;
    private DamageUIPresenter _damageUIPresenter;
    private StageIndexPresenter _stageIndexPresenter;

    #endregion


    #region Public Methods

    public void Init(GameContext gameContext, GameConfigSO gameConfigSO)
    {
        _gameContext = gameContext;
        _gameConfigSO = gameConfigSO;
    }

    public void Compose()
    {
        DisposeAll();

        ConstructSpawners();

        ConstructPresenters();
        InitializePresenters();
        ActivatePresenters();
    }

    #endregion


    #region Unity Methods

    private void Update()
    {
        _skillLoadoutPresenter?.Tick(Time.unscaledTime);
    }

    private void OnDestroy()
    {
        DisposeAll();
    }

    #endregion


    #region Private Methods

    private void ConstructSpawners()
    {
        ObjectPool<DamageTextView> damagePool = new(
            _poolParent,
            _damageTextViewPrefab,
            10
            );
        DamageTextSpawner damageTextSpawner = new(damagePool, _spawnPoint);




        _poolHub = new()
        {
            DamageTextSpawner = damageTextSpawner,
        };
    }


    private void ConstructPresenters()
    {
        _upgradePanelPresenter = new(
            _upgradePanelView,
            _gameConfigSO.UpgradeConfigSO.UpgradeDefs,
            _gameContext.UpgradeManager,
            _gameContext.WalletManager,
            _gameContext.PurchaseManager
            );
        _disposables.Add(_upgradePanelPresenter);

        _skillPanelPresenter = new(
            _skillPanelView,
            _gameConfigSO.SkillConfigSO.SkillDefs,
            _gameContext.SkillManager,
            _gameContext.WalletManager,
            _gameContext.PurchaseManager
            );
        _disposables.Add(_skillPanelPresenter);

        _skillLoadoutPresenter = new(
            _gameContext.SkillManager,
            _gameContext.CombatManager,
            _gameConfigSO.SkillConfigSO,
            _skillLoadoutItemViews
            );
        _disposables.Add(_skillLoadoutPresenter);

        _manualInputPresenter = new(
            _manualAttackButton,
            _gameContext.CombatManager
            );
        _disposables.Add(_manualInputPresenter);

        _damageUIPresenter = new(
            _gameContext.CombatManager,
            _poolHub.DamageTextSpawner,
            _spawnPoint
            );
        _disposables.Add(_damageUIPresenter);

        _monsterHpUIPresenter = new(
            _monsterHpView,
            _gameContext.StageManager
            );
        _disposables.Add(_monsterHpUIPresenter);

        _stageIndexPresenter = new(
            _stageIndexView,
            _gameContext.StageManager
            );
        _disposables.Add(_stageIndexPresenter);
    }

    private void InitializePresenters()
    {
        // C# Presenters
        _upgradePanelPresenter.Initialize();
        _skillPanelPresenter.Initialize();
        _skillLoadoutPresenter.Initialize();
        _manualInputPresenter.Initialize();
        _damageUIPresenter.Initialize();
        _monsterHpUIPresenter.Initialize();
        _stageIndexPresenter.Initialize();

        // Mono Behaviour Presenters
        _skillUIOrchestrator.Initialize(
            _gameConfigSO.SkillConfigSO,
            _gameContext.SkillManager,
            _gameContext.SaveLoadManager
            );
    }

    private void ActivatePresenters()
    {
        _upgradePanelPresenter.Activate();
        _skillPanelPresenter.Activate();
        _skillLoadoutPresenter.Activate();
        _manualInputPresenter.Activate();
        _skillUIOrchestrator.Activate();
        _damageUIPresenter.Activate();
        _monsterHpUIPresenter.Activate();
        _stageIndexPresenter.Activate();
    }

    private void DisposeAll()
    {
        for (int i = 0; i < _disposables.Count; i++)
        {
            _disposables[i]?.Dispose();
        }
        _disposables.Clear();
    }

    #endregion
}
