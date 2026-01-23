using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : PooledObject<T>
{
    private readonly Stack<T> _stack;
    private readonly T _prefab;
    private readonly Transform _poolRoot;

    public ObjectPool(Transform parent, T prefab, int initSize = 5)
    {
        _stack = new(initSize);
        _prefab = prefab;

        GameObject go = new($"{prefab.name} Pool");
        _poolRoot = go.transform;
        _poolRoot.SetParent(parent, false);

        for (int i = 0; i < initSize; i++)
        {
            Create();
        }
    }

    private void Create()
    {
        T obj = Object.Instantiate(_prefab, _poolRoot);
        obj.PooledInit(this);
        PushPool(obj);
    }

    public T PopPool(Transform parent = null)
    {
        if (_stack.Count == 0)
        {
            Create();
        }

        T obj = _stack.Pop();
        obj.gameObject.SetActive(true);

        if (parent != null)
        {
            obj.transform.SetParent(parent, false);
        }

        if (obj is IPoolable p)
        {
            p.OnPop();
        }

        return obj;
    }

    public void PushPool(T obj)
    {
        if (obj is IPoolable p)
        {
            p.OnPush();
        }

        obj.transform.SetParent(_poolRoot, false);
        obj.gameObject.SetActive(false);
        _stack.Push(obj);
    }
}
