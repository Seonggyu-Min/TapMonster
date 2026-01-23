using System;
using UnityEngine;

public class DamageUIPresenter : IDisposable
{
    private readonly CombatManager _combatManager;
    private readonly DamageTextSpawner _spawner;
    private readonly RectTransform _spawnPoint;

    private bool _active;

    public DamageUIPresenter(
        CombatManager combatManager,
        DamageTextSpawner spawner,
        RectTransform spawnPoint
        )
    {
        _combatManager = combatManager;
        _spawner = spawner;
        _spawnPoint = spawnPoint;
    }

    public void Initialize() { /*no op*/ }

    public void Activate()
    {
        if (_active) return;
        _active = true;

        if (_combatManager != null)
        {
            _combatManager.OnHit += HandleHit;
        }
    }

    public void Dispose()
    {
        if (!_active) return;
        _active = false;

        if (_combatManager != null)
        {
            _combatManager.OnHit -= HandleHit;
        }
    }

    private void HandleHit(DamageResult r)
    {
        if (_spawnPoint == null) return;

        string text = BigNumberFormatter.ToString(r.FinalDamage);
        _spawner.Spawn(text, _spawnPoint);
    }
}
