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
        if (!r.IsSuccess) return; // 실패한 공격이면 표시 안함
        if (r.AppliedDamage <= BigNumber.Zero) return;
        if (_spawnPoint == null) return;

        // TODO: 크리티컬과 분리 혹은 스킬도
        // if (r.IsCritical) / if (r.Source == DamageSource.Skill) 이거 쓰면 될 듯

        string text = BigNumberFormatter.ToString(r.AppliedDamage);
        _spawner.Spawn(text, _spawnPoint);
    }
}
