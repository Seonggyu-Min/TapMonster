using System;
using System.Collections.Generic;
using UnityEngine;

// TODO: 풀링해서 새로고침 시 생성 삭제하지 말기
// TODO: 가상화로 스크롤 최적화
public class BottomPanelView<TItemView, TData> : MonoBehaviour
    where TItemView : BottomItemView<TData>
    where TData : struct, IBottomItemData
{
    public event Action<int> OnItemClicked;
    public event Action OnOpened;
    public event Action OnClosed;

    [SerializeField] protected Transform _content;
    [SerializeField] protected TItemView _itemPrefab;

    protected readonly Dictionary<int, TItemView> _viewsById = new();

    public void BuildList(IReadOnlyList<TData> items)
    {
        Clear();

        for (int i = 0; i < items.Count; i++)
        {
            TData data = items[i];

            TItemView view = Instantiate(_itemPrefab, _content);
            view.Bind(data.Id);
            view.OnClicked += HandleItemClicked;
            view.Set(data);

            _viewsById[data.Id] = view;
        }
    }

    public void UpdateItem(TData item)
    {
        if (_viewsById.TryGetValue(item.Id, out TItemView view))
            view.Set(item);
    }

    public void UpdateAllItems(IReadOnlyList<TData> items)
    {
        for (int i = 0; i < items.Count; i++)
            UpdateItem(items[i]);
    }

    protected void HandleItemClicked(int id) => OnItemClicked?.Invoke(id);

    private void OnEnable()
    {
        OnOpened?.Invoke();
    }

    public void OnDisable()
    {
        OnClosed?.Invoke();
    }

    protected virtual void Clear()
    {
        foreach (var kv in _viewsById)
        {
            if (kv.Value != null)
                kv.Value.OnClicked -= HandleItemClicked;
        }
        _viewsById.Clear();

        for (int i = _content.childCount - 1; i >= 0; i--)
        {
            Destroy(_content.GetChild(i).gameObject);
        }
    }
}
