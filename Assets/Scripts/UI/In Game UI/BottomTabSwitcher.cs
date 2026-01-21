using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TabType
{
    Upgrade,
    Skill,
    Relic,
}

[Serializable]
public class TapEntry
{
    public TabType TabType;
    public GameObject TabContent;
}

[Serializable]
public class TapButtonEntry
{
    public TabType TabType;
    public Button TabButton;
}


public class BottomTabSwitcher : MonoBehaviour
{
    [SerializeField] private List<TapEntry> _tabEntries;
    [SerializeField] private List<TapButtonEntry> _tabButtonEntries;

    private const LogCategory CurrentCategory = LogCategory.UI;


    private void OnEnable()
    {
        foreach (TapButtonEntry buttonEntry in _tabButtonEntries)
        {
            Button button = buttonEntry.TabButton;
            if (button == null) continue;

            TabType tabType = buttonEntry.TabType;
            button.onClick.AddListener(() => SwitchTab(tabType));
        }

        // 초기 탭 설정
        if (_tabEntries.Count > 0)
        {
            SwitchTab(_tabEntries[0].TabType);
        }
    }

    private void OnDisable()
    {
        foreach (TapButtonEntry buttonEntry in _tabButtonEntries)
        {
            Button button = buttonEntry.TabButton;
            if (button == null) continue;

            button.onClick.RemoveAllListeners();
        }
    }

    public void SwitchTab(TabType tabType)
    {
        foreach (var entry in _tabEntries)
        {
            bool isActive = entry.TabType == tabType;
            entry.TabContent.SetActive(isActive);
        }
    }
}
