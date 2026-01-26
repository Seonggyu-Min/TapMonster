using System;

public class UpgradeManager : IStatContributor
{
    private readonly UpgradeService _upgradeService;
    private readonly GameConfigSO _gameConfigSO;
    private PurchaseManager _purchaseManager;
    private ISaveMark _saveMark;

    private bool _activated;

    public event Action OnChanged;

    public event Action<int, int> OnUpgradeLevelChanged
    {
        add => _upgradeService.OnUpgradeLevelChanged += value;
        remove => _upgradeService.OnUpgradeLevelChanged -= value;
    }


    public UpgradeManager(
        UpgradeService upgradeService,
        GameConfigSO gameConfigSO
        )
    {
        _upgradeService = upgradeService;
        _gameConfigSO = gameConfigSO;
    }
    public void Initialize(ISaveMark saveMark, PurchaseManager purchaseManager)
    {
        _purchaseManager = purchaseManager;
        _saveMark = saveMark;
    }
    public void Activate()
    {
        if (_activated) return;
        _activated = true;

        _upgradeService.OnUpgradeLevelChanged += HandleUpgradeChanged;
    }
    public void Deactivate()
    {
        if (! _activated) return;
        _activated = false;

        _upgradeService.OnUpgradeLevelChanged -= HandleUpgradeChanged;
    }

    public int GetLevel(int upgradeId)
    {
        return _upgradeService.GetLevel(upgradeId);
    }

    public Cost GetNextCost(int upgradeId)
    {
        return new Cost(
            CurrencyId.Gold,
            _upgradeService.GetLevelUpCost(
                _gameConfigSO,
                upgradeId,
                _upgradeService.GetLevel(upgradeId) + 1));
    }

    public bool TryUpgrade(int upgradeId)
    {
        int nextLevel = _upgradeService.GetLevel(upgradeId) + 1;
        Cost cost = new Cost(
            CurrencyId.Gold,
            _upgradeService.GetLevelUpCost(
                _gameConfigSO,
                upgradeId,
                nextLevel));

        var result = _purchaseManager.TryPay(cost);
        if (result != PurchaseResult.Success) return false;

        _upgradeService.AddLevel(upgradeId, +1);

        _saveMark.MarkDirty(SaveDirtyFlags.Upgrade);
        _saveMark.RequestSave();

        return true;
    }


    public void Contribute(ref PlayerStatBuildContext buildContext)
    {
        _upgradeService.ApplyToStat(ref buildContext, _gameConfigSO);
    }

    private void HandleUpgradeChanged(int id, int level) => OnChanged?.Invoke();
}
