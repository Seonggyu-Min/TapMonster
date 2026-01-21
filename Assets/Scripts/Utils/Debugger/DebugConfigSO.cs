using EditorAttributes;
using System;
using UnityEngine;


public enum LogCategory
{
    None,
    Firebase,
    PUN,
    GameLogic,
    UI,
    Bootstrap,
    Addressables,
    Upgrade,
    Skill,
    Relic,
    Reward,
    Wallet,
    Purchase,
    AdMob,
    Debug,
    Animation,
    ETC
}


[CreateAssetMenu(menuName = "ScriptableObjects/DebugConfig")]
public class DebugConfigSO : ScriptableObject
{
    [field: SerializeField] public bool EnableLog { get; private set; } = true;


    [Serializable]
    public struct CategorySetting
    {
        public LogCategory LogCategory;
        public Color Color;
        public bool IsEnabled;
    }

    [SerializeField] private CategorySetting[] categorySettings;

    public Color GetColor(LogCategory category)
    {
        foreach (var c in categorySettings)
        {
            if (c.LogCategory == category)
            {
                return c.Color;
            }
        }

        return Color.white;
    }

    public bool IsCategoryEnabled(LogCategory category)
    {
        foreach (var c in categorySettings)
        {
            if (c.LogCategory == category)
            {
                return c.IsEnabled;
            }
        }
        return true;
    }

    #region Editor Buttons

    [Button("Set All Enabled")]
    public void SetAllEnabled()
    {
        for (int i = 0; i < categorySettings.Length; i++)
        {
            var c = categorySettings[i];
            c.IsEnabled = true;
            categorySettings[i] = c;
        }
    }

    [Button("Set All Disabled")]
    public void SetAllDisabled()
    {
        for (int i = 0; i < categorySettings.Length; i++)
        {
            var c = categorySettings[i];
            c.IsEnabled = false;
            categorySettings[i] = c;
        }
    }

    #endregion
}
