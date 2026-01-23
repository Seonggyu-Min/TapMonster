using UnityEngine;


// TODO: 슬롯 동적 변경
// MB가 아니어도 괜찮을 듯 Composer가 통일하게 관리하니까
public class SkillUIOrchestrator : MonoBehaviour
{
    #region Fields and Properties and Constructor

    [SerializeField] private SkillSlotView[] _equippedSlots;
    [SerializeField] private SkillSlotView[] _inventorySlots;

    private SkillManager _skillManager;
    private SaveLoadManager _saveLoadManager;
    private SkillConfigSO _skillConfigSO;

    private bool _active;

    #endregion


    #region Unity Methods

    private void OnDestroy()
    {
        if (!_active) return;
        _active = false;

        if (_skillManager != null)
        {
            _skillManager.OnSkillSlotChanged -= OnModelChanged;
        }
    }

    #endregion


    #region Public Methods

    public void Initialize(
        SkillConfigSO skillConfigSO,
        SkillManager skillManager,
        SaveLoadManager saveLoadManager
        )
    {
        _skillConfigSO = skillConfigSO;
        _skillManager = skillManager;
        _saveLoadManager = saveLoadManager;
    }
    public void Activate()
    {
        if (_active) return;
        _active = true;

        if (_skillManager != null)
        {
            _skillManager.OnSkillSlotChanged -= OnModelChanged;
            _skillManager.OnSkillSlotChanged += OnModelChanged;
        }

        InitAllSlots();
        RefreshAll();
    }

    public bool ApplyRule(SkillSlotView source, SkillSlotView target)
    {
        if (source == null || target == null) return false;
        if (ReferenceEquals(source, target)) return false;

        int sourceId = GetSkillIdFrom(source);
        if (sourceId == SkillId.None) return false;   // 빈 슬롯 드래그 금지

        // 1) Inventory <-> Inventory : 위치 교환 금지
        if (source.Kind == SlotKind.Inventory && target.Kind == SlotKind.Inventory) return false;

        // 2) Equipped <-> Equipped : 위치 교환 허용
        if (source.Kind == SlotKind.Equipped && target.Kind == SlotKind.Equipped)
        {
            _skillManager.SwapEquipped(source.Index, target.Index);
            return true;
        }

        // 3) Inventory -> Equipped 대체 + 중복 금지
        if (source.Kind == SlotKind.Inventory && target.Kind == SlotKind.Equipped)
        {
            if (IsAlreadyEquipped(sourceId, exceptIndex: target.Index))
            {
                // TODO: UI 피드백
                this.PrintLog("이미 장착된 스킬", LogCategory.UI);
                return false;
            }

            _skillManager.ReplaceEquipped(target.Index, sourceId);
            return true;
        }

        // 4) Equipped -> Inventory : 위치 교환 금지
        if (source.Kind == SlotKind.Equipped && target.Kind == SlotKind.Inventory) return false;

        return false;
    }

    public int GetSkillIdFrom(SkillSlotView slot)
    {
        if (slot.Kind == SlotKind.Equipped)
        {
            return _skillManager.GetEquipped(slot.Index);
        }

        // Inventory
        if (slot.Index < _skillManager.Inventory.Count)
        {
            return _skillManager.GetInventory(slot.Index);
        }
        return SkillId.None;
    }

    public void RefreshAll()
    {
        // equipped
        for (int i = 0; i < _equippedSlots.Length; i++)
        {
            int id = _skillManager.GetEquipped(i);
            if (id != SkillId.None && _skillConfigSO.TryGet(id, out var def))
            {
                _equippedSlots[i].Render(def.Icon);
            }
            else
            {
                _equippedSlots[i].Render(null);
            }
        }

        // inventory
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            _inventorySlots[i].SetIndex(i);

            int id = (i < _skillManager.Inventory.Count) ? _skillManager.GetInventory(i) : SkillId.None;
            if (id != SkillId.None && _skillConfigSO.TryGet(id, out var def))
            {
                _inventorySlots[i].Render(def.Icon);
            }
            else
            {
                _inventorySlots[i].Render(null);
            }
        }
    }

    public Sprite GetIconOrNull(int skillId)
    {
        if (skillId == SkillId.None) return null;
        return _skillConfigSO.TryGet(skillId, out var def) ? def.Icon : null;
    }

    public void TryApplyRuleAndSave(SkillSlotView source, SkillSlotView target)
    {
        bool changed = ApplyRule(source, target);
        if (!changed) return;

        _saveLoadManager?.MarkDirty(SaveDirtyFlags.SkillSlots);
        _saveLoadManager?.RequestSave();
    }

    #endregion


    #region Private Methods

    private void OnModelChanged(SkillSlotChangeKind kind) => RefreshAll();

    private void InitAllSlots()
    {
        for (int i = 0; i < _equippedSlots.Length; i++)
        {
            _equippedSlots[i].Init(SlotKind.Equipped, i, this);
        }
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            _inventorySlots[i].Init(SlotKind.Inventory, i, this);
        }
    }

    private bool IsAlreadyEquipped(int skillId, int exceptIndex)
    {
        for (int i = 0; i < _equippedSlots.Length; i++)
        {
            if (i == exceptIndex) continue;
            if (_skillManager.GetEquipped(i) == skillId) return true;
        }
        return false;
    }

    #endregion
}
