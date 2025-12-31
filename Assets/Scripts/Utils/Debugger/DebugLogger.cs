using UnityEngine;

public static class DebugLogger
{
    public static void PrintLog<T>(this T type, string log, LogType logType = LogType.Log, Color? color = null)
    {
        if (!DebugConfig.Current.EnableLog) return;

        // 지정된 색이 없으면 기본은 white
        var c = color ?? Color.white;
        string hex = ColorUtility.ToHtmlStringRGB(c);

        string prefix = $"[<color=#{hex}>{type}</color>]:";

        switch (logType)
        {
            case LogType.Error:
                Debug.LogError($"{prefix} {log}");
                break;
            case LogType.Warning:
                Debug.LogWarning($"{prefix} {log}");
                break;
            case LogType.Log:
                Debug.Log($"{prefix} {log}");
                break;
            default:
                Debug.Log($"{prefix} {log}");
                break;
        }
    }
}
