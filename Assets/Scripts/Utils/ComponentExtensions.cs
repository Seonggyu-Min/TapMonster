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

    public static bool TryBindComponentInChildren<T>(
        this Component owner,
        ref T field,
        LogCategory category = LogCategory.UI,
        bool includeInactive = true,
        LogType logType = LogType.Error)
        where T : Component
    {
        if (field != null) return true;

        field = owner.GetComponentInChildren<T>(includeInactive);
        if (field != null) return true;

        owner.PrintLog($"{typeof(T).Name} (in children) 컴포넌트를 찾을 수 없습니다.", category, logType);
        return false;
    }

    public static bool TryBindComponentInParent<T>(
        this Component owner,
        ref T field,
        LogCategory category = LogCategory.UI,
        bool includeInactive = true,
        LogType logType = LogType.Error)
        where T : Component
    {
        if (field != null) return true;

        field = owner.GetComponentInParent<T>(includeInactive);
        if (field != null) return true;

        owner.PrintLog($"{typeof(T).Name} (in parent) 컴포넌트를 찾을 수 없습니다.", category, logType);
        return false;
    }
}
