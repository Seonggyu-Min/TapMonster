using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class BottomItemView<TData> : MonoBehaviour where TData : struct, IBottomItemData
{
    [SerializeField] protected Button _button;
    [SerializeField] protected Image _icon;
    [SerializeField] protected TMP_Text _nameText;

    protected int _id;
    public event Action<int> OnClicked;
    private UnityAction _cachedClickAction;


    protected virtual void Awake()
    {
        _cachedClickAction = () => OnClicked?.Invoke(_id);
    }

    protected virtual void OnEnable()
    {
        _button.onClick.AddListener(_cachedClickAction);
    }

    protected virtual void OnDisable()
    {
        _button.onClick.RemoveListener(_cachedClickAction);
    }


    public virtual void Bind(int id)
    {
        _id = id;
    }

    public void Set(TData data)
    {
        // 공통 표시
        _icon.sprite = data.Icon;
        _nameText.text = data.Name;

        // 개별 표시
        ApplySpecific(data);
    }

    protected abstract void ApplySpecific(TData data);
}
