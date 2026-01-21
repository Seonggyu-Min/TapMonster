using System;
using UnityEngine;

public class SkillLoadoutPresenter : IDisposable
{
    private SkillManager _skillManager;
    private SkillConfigSO _skillConfigSO;
    private SkillLoadoutItemView[] _views;

    private float _now;


    public SkillLoadoutPresenter(
        SkillManager skillManager,
        SkillConfigSO skillConfigSO,
        SkillLoadoutItemView[] views
        )
    {
        _skillManager = skillManager;
        _skillConfigSO = skillConfigSO;
        _views = views;
    }

    public void Initialize()
    {
        _skillManager.OnSkillUsed += OnSkillUsed;
        _skillManager.OnSkillSlotChangeEvent += OnSkillSlotChanged;

        for (int i = 0; i < _views.Length; i++)
        {
            _views[i].Init(i);
            _views[i].OnClickedSlot += OnClickedSlot;
        }

        RefreshAllEquipped();
    }

    public void Dispose()
    {
        _skillManager.OnSkillUsed -= OnSkillUsed;
        _skillManager.OnSkillSlotChangeEvent -= OnSkillSlotChanged;

        for (int i = 0; i < _views.Length; i++)
        {
            if (_views[i] != null)
            {
                _views[i].OnClickedSlot -= OnClickedSlot;
            }
        }
    }

    private Sprite GetSprite(int skillId)
    {
        if (_skillConfigSO.TryGet(skillId, out var def)) return def.Icon;
        return null;
    }

    private void RefreshAllEquipped()
    {
        for (int i = 0; i < _views.Length; i++)
        {
            int skillId = _skillManager.GetEquipped(i);
            Sprite icon = GetSprite(skillId);
            _views[i].SetSkill(skillId, icon);
        }
    }

    private void OnClickedSlot(int slotIndex)
    {
        int skillId = _views[slotIndex].SkillId;

        if (skillId == SkillId.None) return;

        _skillManager.TryUseSkill(skillId);
    }

    private void OnSkillUsed(SkillUseEvent e)
    {
        // 사용된 스킬의 슬롯 찾기
        for (int i = 0; i < _views.Length; i++)
        {
            if (_views[i].SkillId != e.SkillId)
                continue;

            // 시작 UI 반영
            _views[i].SetCooldownNormalized(1f);
            _views[i].SetCooldownSeconds(Mathf.CeilToInt(e.CooldownSeconds));
        }
    }

    private void OnSkillSlotChanged(SkillSlotChangeEvent ev)
    {
        if (ev.SlotType != SlotType.Equipped) return;
        if (ev.SlotIndex < 0 || ev.SlotIndex >= _views.Length) return;

        Sprite icon = GetSprite(ev.SkillId);
        _views[ev.SlotIndex].SetSkill(ev.SkillId, icon);

        float remain = _skillManager.GetCooldownRemaining(ev.SkillId, _now);

        if (remain > 0f)
        {
            float duration = _skillManager.GetSkillCooldownSeconds(ev.SkillId);

            _views[ev.SlotIndex].SetCooldownNormalized(remain / duration);
            _views[ev.SlotIndex].SetCooldownSeconds(Mathf.CeilToInt(remain));
        }
        else
        {
            _views[ev.SlotIndex].SetCooldownNormalized(0f);
            _views[ev.SlotIndex].SetCooldownSeconds(0);
        }
    }

    // Composer의 Update에서 호출됨
    public void Tick(float now)
    {
        _now = now;

        for (int i = 0; i < _views.Length; i++)
        {
            int skillId = _views[i].SkillId;

            if (skillId == SkillId.None)
            {
                _views[i].SetCooldownNormalized(0f);
                _views[i].SetCooldownSeconds(0);
                continue;
            }

            float remain = _skillManager.GetCooldownRemaining(skillId, now);

            if (remain <= 0f)
            {
                _views[i].SetCooldownNormalized(0f);
                _views[i].SetCooldownSeconds(0);
                continue;
            }

            float dur = Mathf.Max(0.0001f, _skillManager.GetSkillCooldownSeconds(skillId));

            _views[i].SetCooldownNormalized(remain / dur);
            _views[i].SetCooldownSeconds(Mathf.CeilToInt(remain));
        }
    }
}
