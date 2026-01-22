using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private NormalMonster _monsterPrefab;

    private NormalMonster _spawned;

    

    public void SpawnAndBind(GameContext ctx)
    {
        if (_spawned == null)
            _spawned = Instantiate(_monsterPrefab);

        ctx.StageManager.BindEnemy(_spawned);
        ctx.StageManager.SpawnOrResetEnemy();
    }
}
