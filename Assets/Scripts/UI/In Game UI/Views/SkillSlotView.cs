using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SlotKind
{
    Equipped = 0,
    Inventory = 1
}

public class SkillSlotView : MonoBehaviour, IDropHandler
{
    [Header("Refs")]
    [SerializeField] private SkillDragHandle _dragHandle;
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite emptySprite; // default

    private SlotKind _kind;
    private int _index;
    private SkillUIOrchestrator _skillUIOrchestrator;

    private const LogCategory CurrentCategory = LogCategory.UI;

    public SlotKind Kind => _kind;
    public int Index => _index;


    public void Init(SlotKind slotKind, int index, SkillUIOrchestrator skillUIOrchestrator)
    {
        Setkind(slotKind);
        SetIndex(index);
        Bind(skillUIOrchestrator);
    }

    public void Bind(SkillUIOrchestrator skillUIOrchestrator) => _skillUIOrchestrator = skillUIOrchestrator;
    public void Setkind(SlotKind kind) => _kind = kind;
    public void SetIndex(int index) => _index = index;



    private void Awake()
    {
        if (_dragHandle == null)
        {
            this.TryBindComponent(ref _dragHandle, CurrentCategory);
        }
        if (iconImage != null)
        {
            iconImage.raycastTarget = true;
        }
    }


    public void Render(Sprite skillIcon)
    {
        if (skillIcon == null)
        {
            if (emptySprite != null)
            {
                iconImage.sprite = emptySprite;
                iconImage.enabled = true;
            }
            else
            {
                iconImage.enabled = false;
            }
            return;
        }

        iconImage.sprite = skillIcon;
        iconImage.enabled = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_skillUIOrchestrator == null) return;
        if (eventData.pointerDrag == null) return;

        SkillDragHandle handle = eventData.pointerDrag.GetComponent<SkillDragHandle>();
        if (handle == null) return;

        SkillSlotView source = handle.SourceSlot;
        _skillUIOrchestrator.TryApplyRuleAndSave(source, this);
    }
}
