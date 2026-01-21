using UnityEngine;

public static class DebugLogger
{
    public static void PrintLog<T>(
        this T type,
        string log,
        LogCategory category = LogCategory.None,
        LogType logType = LogType.Log
        )
    {
        if (!DebugConfig.Current.EnableLog) return;
        if (!DebugConfig.Current.IsCategoryEnabled(category)) return;

        Color color = DebugConfig.Current.GetColor(category);
        string hex = ColorUtility.ToHtmlStringRGB(color);
        string prefix = $"[<color=#{hex}>{category}</color>]";

        switch (logType)
        {
            case LogType.Error:
                Debug.LogError($"{prefix} [{type}]: {log}");
                break;
            case LogType.Warning:
                Debug.LogWarning($"{prefix} [{type}]: {log}");
                break;
            case LogType.Log:
                Debug.Log($"{prefix} [{type}]: {log}");
                break;
            default:
                Debug.Log($"{prefix} [{type}]: {log}");
                break;
        }
    }
}
