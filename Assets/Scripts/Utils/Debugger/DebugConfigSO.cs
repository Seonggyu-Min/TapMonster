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
    Addressables
}


[CreateAssetMenu(menuName = "ScriptableObjects/DebugConfig")]
public class DebugConfigSO : ScriptableObject
{
    [field: SerializeField] public bool EnableLog { get; private set; } = true;


    [Serializable]
    public struct CategoryColor
    {
        public LogCategory LogCategory;
        public Color Color;
    }

    [SerializeField] private CategoryColor[] categoryColors;

    public Color GetColor(LogCategory category)
    {
        foreach (var c in categoryColors)
        {
            if (c.LogCategory == category)
            {
                return c.Color;
            }
        }

        return Color.white;
    }
}
