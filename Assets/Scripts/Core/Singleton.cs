using System;
using UnityEngine;


public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static bool WillDestroyOnSceneLoad { get; protected set; } = false;

    public static T Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<T>();

                if (!_instance)
                {
                    string msg = $"[Singleton<{typeof(T).Name}>] Scene에 인스턴스가 없습니다.";
                    Debug.LogError(msg);
                    throw new InvalidOperationException(msg);
                }

                if (!WillDestroyOnSceneLoad)
                    DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    protected virtual void Awake() => SingletonInit();

    protected void SingletonInit()
    {
        if (_instance && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;

        if (!WillDestroyOnSceneLoad)
            DontDestroyOnLoad(gameObject);
    }

    public static bool TryGetInstance(out T instance)
    {
        if (!_instance) _instance = FindObjectOfType<T>();
        instance = _instance;
        return instance != null;
    }
}