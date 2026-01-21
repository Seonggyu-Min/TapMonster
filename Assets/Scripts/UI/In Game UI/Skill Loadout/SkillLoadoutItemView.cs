using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillLoadoutItemView : MonoBehaviour
{
    [SerializeField] private Button _useButton;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _cooldownMask;   // Filled, Radial 360
    [SerializeField] private TMP_Text _cooldownText;

    private int _skillId;
    private int _slotIndex;

    private const LogCategory CurrentCategory = LogCategory.UI;

    public int SlotIndex => _slotIndex;
    public int SkillId => _skillId;

    public event Action<int> OnClickedSlot; // slotIndex 전달



    private void Awake()
    {
        if (_useButton == null)
        {
            this.TryBindComponent(ref _useButton, CurrentCategory);
        }
    }


    private void OnEnable()
    {
        if (_useButton != null)
        {
            _useButton.onClick.AddListener(OnClickUseButton);
        }
    }

    private void OnDisable()
    {
        if (_useButton != null)
        {
            _useButton.onClick.RemoveListener(OnClickUseButton);
        }
    }

    public void Init(int slotIndex)
    {
        _slotIndex = slotIndex;
        ResetCooldownUI();
    }

    public void SetSkill(int skillId, Sprite icon)
    {
        _skillId = skillId;
        _icon.sprite = icon;

        // 스킬 바뀌면 쿨타임 UI 초기화
        ResetCooldownUI();
    }

    public void SetCooldownNormalized(float t01)
    {
        if (_cooldownMask == null) return;

        _cooldownMask.fillAmount = Mathf.Clamp01(t01);
        _cooldownMask.gameObject.SetActive(_cooldownMask.fillAmount > 0f);
    }

    public void SetCooldownSeconds(int seconds)
    {
        if (_cooldownText == null) return;

        if (seconds <= 0)
        {
            _cooldownText.text = string.Empty;
            _cooldownText.gameObject.SetActive(false);
            return;
        }

        _cooldownText.text = seconds.ToString();
        _cooldownText.gameObject.SetActive(true);
    }

    public void ResetCooldownUI()
    {
        SetCooldownNormalized(0f);
        SetCooldownSeconds(0);
    }

    public void OnClickUseButton()
    {
        OnClickedSlot?.Invoke(_slotIndex);
    }
}
