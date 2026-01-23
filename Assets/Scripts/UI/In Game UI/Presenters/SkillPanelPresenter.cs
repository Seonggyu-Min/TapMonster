using System;
using System.Collections.Generic;

public class SkillPanelPresenter : IDisposable
{
    private SkillPanelView _view;
    private IReadOnlyList<SkillConfigSO.SkillDef> _defs;
    private SkillManager _skillManager;
    private WalletManager _walletManager;
    private PurchaseManager _purchaseManager;

    private Dictionary<int, SkillConfigSO.SkillDef> _defById = new();
    private List<int> _orderedIds = new();

    private bool _active;
    public SkillPanelPresenter(
        SkillPanelView view,
        IReadOnlyList<SkillConfigSO.SkillDef> defs,
        SkillManager skillManager,
        WalletManager walletManager,
        PurchaseManager purchaseManager
        )
    {
        _view = view;
        _defs = defs;
        _skillManager = skillManager;
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
            SkillConfigSO.SkillDef def = _defs[i];
            _defById[def.Id] = def;
            _orderedIds.Add(def.Id);
        }

        // View 이벤트 구독
        _view.OnItemClicked += HandleSkillItemClicked;
        _view.OnOpened += HandleOpened;
        _view.OnClosed += HandleClosed;

        // 변경 시 UI 갱신용 재화 및 스킬 이벤트 구독
        _skillManager.OnSkillLevelChanged += HandleLevelChanged;
        _walletManager.OnCurrencyChanged += HandleGoldChanged;

        // 최초 리스트 빌드
        List<SkillItemVM> vms = BuildAllVMs();
        _view.BuildList(vms);
    }


    public void Dispose()
    {
        if (!_active) return;
        _active = false;

        _view.OnItemClicked -= HandleSkillItemClicked;

        _skillManager.OnSkillLevelChanged -= HandleLevelChanged;
        _walletManager.OnCurrencyChanged -= HandleGoldChanged;
    }

    private void HandleOpened()
    {
        _view.UpdateAllItems(BuildAllVMs());
    }

    private void HandleClosed()
    {

    }

    private void HandleSkillItemClicked(int id)
    {
        _skillManager.TryLevelUpSkill(id);
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
        if (!_defById.TryGetValue(id, out SkillConfigSO.SkillDef def))
            return;
            
        SkillItemVM vm = BuildVM(def);
        _view.UpdateItem(vm);
    }

    private void RefreshAll()
    {
        _view.UpdateAllItems(BuildAllVMs());
    }


    private List<SkillItemVM> BuildAllVMs()
    {
        List<SkillItemVM> list = new(_orderedIds.Count);

        for (int i = 0; i < _orderedIds.Count; i++)
        {
            int id = _orderedIds[i];
            if (!_defById.TryGetValue(id, out SkillConfigSO.SkillDef def))
                continue;

            SkillItemVM vm = BuildVM(def);
            list.Add(vm);
        }

        return list;
    }

    private SkillItemVM BuildVM(SkillConfigSO.SkillDef def)
    {
        int level = _skillManager.GetLevel(def.Id);
        Cost nextCost = _skillManager.GetNextCost(def.Id);
        bool canBuy = _purchaseManager.CanPay(nextCost);

        return new SkillItemVM(
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
