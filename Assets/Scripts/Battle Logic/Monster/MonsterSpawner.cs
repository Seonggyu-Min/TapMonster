using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private NormalMonster _normalPrefab;
    [SerializeField] private NormalMonster _bossPrefab;
    [SerializeField] private Transform _root;

    private NormalMonster _spawned;


    // TODO: 보스도 받게 바꿔야 됨
    public NormalMonster Spawn(StageManager stageManager, bool resetHp)
    {
        NormalMonster prefab = stageManager.CurrentTargetType == TargetType.Boss
            ? _bossPrefab : _normalPrefab;

        if (_spawned == null || _spawned.gameObject.name.StartsWith(prefab.name) == false)
        {
            if (_spawned != null) Destroy(_spawned.gameObject);
            _spawned = Instantiate(prefab, _root);
        }

        stageManager.BindEnemyView(_spawned);
        if (resetHp)
        {
            stageManager.SpawnOrResetEnemy();
        }

        return _spawned;
    }
}
