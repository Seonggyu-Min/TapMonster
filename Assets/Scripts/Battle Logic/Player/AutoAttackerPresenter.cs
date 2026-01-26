using System;
using System.Collections;
using UnityEngine;

public class AutoAttackerPresenter : IDisposable
{
    private readonly AutoAttackerView _autoAttackerView;
    private readonly CombatCoordinator _combatCoordinator;
    private readonly CombatConfigSO _combatConfigSO;
    private readonly AutoAttackConfigSO _autoAttackConfigSO;

    private readonly MonoBehaviour _coroutineHost;
    private Coroutine _loop;
    private WaitForSeconds _wait;
    private bool _activated;


    public AutoAttackerPresenter(
        AutoAttackerView autoAttackerView,
        CombatCoordinator combatCoordinator,
        CombatConfigSO combatConfigSO,
        AutoAttackConfigSO autoAttackConfigSO,
        MonoBehaviour coroutineHost
    )
    {
        _autoAttackerView = autoAttackerView;
        _combatCoordinator = combatCoordinator;
        _combatConfigSO = combatConfigSO;
        _autoAttackConfigSO = autoAttackConfigSO;
        _coroutineHost = coroutineHost;
    }
    public void Initialize() { /* no op */ }
    public void Activate()
    {
        if (_activated) return;
        _activated = true;

        _wait = new(_combatConfigSO.AutoAttackInterval);
        _loop = _coroutineHost.StartCoroutine(AutoLoop());
    }
    public void Dispose()
    {
        if (!_activated) return;
        _activated = false;

        if (_loop != null)
        {
            _coroutineHost.StopCoroutine(_loop);
            _loop = null;
        }
        if (_wait != null)
        {
            _wait = null;
        }
    }

    private IEnumerator AutoLoop()
    {
        while (_activated)
        {
            yield return _wait;
            // 데미지 요청
            _combatCoordinator.TryAuto();
            // 연출
            _autoAttackerView.Flash(_autoAttackConfigSO.FlashDurationSeconds);
        }
    }
}
