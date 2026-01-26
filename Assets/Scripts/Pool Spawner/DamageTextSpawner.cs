using UnityEngine;

public class DamageTextSpawner
{
    private readonly ObjectPool<DamageTextView> _pool;
    private readonly RectTransform _root;

    public DamageTextSpawner(ObjectPool<DamageTextView> pool, RectTransform root)
    {
        _pool = pool;
        _root = root;
    }

    public void Spawn(string text, RectTransform spawnPoint, bool isCritical)
    {
        if (spawnPoint == null) return;

        DamageTextView view = _pool.PopPool(_root);
        Vector2 anchoredPos = (Vector2)_root.InverseTransformPoint(spawnPoint.position);
        view.Show(text, anchoredPos, isCritical);
    }
}
