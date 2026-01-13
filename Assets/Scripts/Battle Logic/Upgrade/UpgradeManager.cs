using System;
using Unity.VisualScripting.FullSerializer;

public class UpgradeManager : IStatContributor
{
    private UpgradeService _upgradeService;
    private GameConfigSO _gameConfigSO;
    private PurchaseManager _purchaseManager;
    private ISaveMark _saveMark;

    public UpgradeManager(
        UpgradeService upgradeService,
        GameConfigSO gameConfigSO,
        PurchaseManager purchaseManager,
        ISaveMark saveMark
        )
    {
        _upgradeService = upgradeService;
        _gameConfigSO = gameConfigSO;
        _purchaseManager = purchaseManager;
        _saveMark = saveMark;
    }

    public bool TryUpgrade(int upgradeId)
    {
        int nextLevel = _upgradeService.GetLevel(upgradeId) + 1;
        BigNumber cost = _upgradeService.GetUpgradeCost(_gameConfigSO, upgradeId, nextLevel);

        var result = _purchaseManager.TryPay(new Cost(CurrencyId.Gold, cost));
        if (result != PurchaseResult.Success) return false;

        _upgradeService.AddLevel(upgradeId, +1);

        _saveMark.MarkDirty(SaveDirtyFlags.Upgrade);
        _saveMark.RequestSave();

        return true;
    }

    // TODO: 업그레이드 가격 조회

    public void Contribute(ref PlayerStatBuildContext buildContext)
    {
        _upgradeService.ApplyToStat(ref buildContext, _gameConfigSO);
    }
}
