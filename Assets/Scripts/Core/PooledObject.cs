using UnityEngine;

public abstract class PooledObject<T> : MonoBehaviour where T : PooledObject<T>
{
    public ObjectPool<T> ObjPool { get; private set; }

    public void PooledInit(ObjectPool<T> pool)
    {
        ObjPool = pool;
    }

    public void ReturnPool()
    {
        ObjPool.PushPool((T)this);
    }
}
