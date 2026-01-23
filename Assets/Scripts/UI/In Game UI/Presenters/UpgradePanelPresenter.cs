using System;
using System.Collections.Generic;

public class UpgradePanelPresenter : IDisposable
{
    private UpgradePanelView _view;
    private IReadOnlyList<UpgradeConfigSO.UpgradeDef> _defs;
    private UpgradeManager _upgradeManager;
    private WalletManager _walletManager;
    private PurchaseManager _purchaseManager;

    private Dictionary<int, UpgradeConfigSO.UpgradeDef> _defById = new();
    private List<int> _orderedIds = new();

    private bool _active;

    public UpgradePanelPresenter(
        UpgradePanelView view,
        IReadOnlyList<UpgradeConfigSO.UpgradeDef> defs,
        UpgradeManager upgradeManager,
        WalletManager walletManager,
        PurchaseManager purchaseManager
        )
    {
        _view = view;
        _defs = defs;
        _upgradeManager = upgradeManager;
        _walletManager = walletManager;
        _purchaseManager = purchaseManager;
    }
    public void Initialize() { /*no op*/ }
    public void Activate()
    {
        if (_active) return;
        _active = true;

        // 캐시 및 순서 빌드
        _defById.Clear();
        _orderedIds.Clear();

        for (int i = 0; i < _defs.Count; i++)
        {
            UpgradeConfigSO.UpgradeDef def = _defs[i];
            _defById[def.Id] = def;
            _orderedIds.Add(def.Id);
        }

        // View 이벤트 구독
        _view.OnItemClicked += HandleUpgradeItemClicked;
        _view.OnOpened += HandleOpened;
        _view.OnClosed += HandleClosed;

        // 변경 시 UI 갱신용 재화 및 업그레이드 이벤트 구독
        _upgradeManager.OnUpgradeLevelChanged += HandleLevelChanged;
        _walletManager.OnCurrencyChanged += HandleGoldChanged;

        // 최초 리스트 빌드
        List<UpgradeItemVM> vms = BuildAllVMs();
        _view.BuildList(vms);
    }

    public void Dispose()
    {
        if (!_active) return;
        _active = false;

        _view.OnItemClicked -= HandleUpgradeItemClicked;
        _view.OnOpened -= HandleOpened;
        _view.OnClosed -= HandleClosed;

        _upgradeManager.OnUpgradeLevelChanged -= HandleLevelChanged;
        _walletManager.OnCurrencyChanged -= HandleGoldChanged;
    }


    private void HandleOpened()
    {
        _view.UpdateAllItems(BuildAllVMs());
    }

    private void HandleClosed()
    {

    }

    private void HandleUpgradeItemClicked(int id)
    {
        bool bought = _upgradeManager.TryUpgrade(id);
    }

    private void HandleLevelChanged(int id, int level)
    {
        RefreshOne(id);
    }

    private void HandleGoldChanged(CurrencyId id, BigNumber amount)
    {
        RefreshAll();   // 필요하면 가능한 가격만 갱신하도록 필터링 해도 될 듯
    }

    private void RefreshOne(int id)
    {
        if (!_defById.TryGetValue(id, out UpgradeConfigSO.UpgradeDef def))
            return;

        UpgradeItemVM vm = BuildVM(def);
        _view.UpdateItem(vm);
    }

    private void RefreshAll()
    {
        _view.UpdateAllItems(BuildAllVMs());
    }



    private List<UpgradeItemVM> BuildAllVMs()
    {
        List<UpgradeItemVM> list = new(_orderedIds.Count);

        for (int i = 0; i < _orderedIds.Count; i++)
        {
            int id = _orderedIds[i];
            if (!_defById.TryGetValue(id, out UpgradeConfigSO.UpgradeDef def))
                continue;

            UpgradeItemVM vm = BuildVM(def);
            list.Add(vm);
        }

        return list;
    }

    private UpgradeItemVM BuildVM(UpgradeConfigSO.UpgradeDef def)
    {
        int level = _upgradeManager.GetLevel(def.Id);
        Cost nextCost = _upgradeManager.GetNextCost(def.Id);
        bool canBuy = _purchaseManager.CanPay(nextCost);

        return new UpgradeItemVM(
            id: def.Id,
            name: def.Name,
            icon: def.Icon,
            level: level,
            maxLevel: def.MaxLevel,
            nextCost: nextCost,
            canBuy: canBuy
        );
    }
}
