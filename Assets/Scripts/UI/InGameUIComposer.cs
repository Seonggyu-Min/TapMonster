using System;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIComposer : MonoBehaviour
{
    [Header("Upgrade Panel Refs")]
    [SerializeField] private UpgradePanelView _upgradePanelView;
    
    [Header("Skill Panel Refs")]
    [SerializeField] private SkillPanelView _skillPanelView;

    [Header("Skill Inventory Panel Refs")]
    [SerializeField] private SkillUIOrchestrator _skillUIOrchestrator;


    private List<IDisposable> _disposables = new();
    private GameContext _gameContext;
    private GameConfigSO _gameConfigSO;


    public void Init(GameContext gameContext, GameConfigSO gameConfigSO)
    {
        _gameContext = gameContext;
        _gameConfigSO = gameConfigSO;
    }

    public void Compose()
    {
        DisposeAll();

        // --- C# Presenters ---
        var upgradePresenter = new UpgradePanelPresenter(
            _upgradePanelView,
            _gameConfigSO.UpgradeConfigSO.UpgradeDefs,
            _gameContext.UpgradeManager,
            _gameContext.WalletManager,
            _gameContext.PurchaseManager
        );
        upgradePresenter.Initialize();
        _disposables.Add(upgradePresenter);

        var skillPresenter = new SkillPanelPresenter(
            _skillPanelView,
            _gameConfigSO.SkillConfigSO.SkillDefs,
            _gameContext.SkillManager,
            _gameContext.WalletManager,
            _gameContext.PurchaseManager
        );
        skillPresenter.Initialize();
        _disposables.Add(skillPresenter);


        // --- Mono Behaviours ---
        _skillUIOrchestrator.Init(
            _gameConfigSO.SkillConfigSO,
            _gameContext.GameStateModel.SkillSlotModel,
            _gameContext.SaveLoadManager
            );
    }

    private void OnDestroy()
    {
        DisposeAll();
    }

    private void DisposeAll()
    {
        for (int i = 0; i < _disposables.Count; i++)
        {
            _disposables[i]?.Dispose();
        }
        _disposables.Clear();
    }
}
