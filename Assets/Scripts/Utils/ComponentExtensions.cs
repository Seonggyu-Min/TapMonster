using UnityEngine;

public static class ComponentExtensions
{
    public static bool TryBindComponent<T>(
        this Component owner,
        ref T field,
        LogCategory category,
        LogType logType = LogType.Error)
        where T : Component
    {
        if (field != null) return true;
        if (owner.TryGetComponent(out field)) return true;

        owner.PrintLog($"{typeof(T).Name} 컴포넌트를 찾을 수 없습니다.", category, logType);
        return false;
    }
}
