using System;
using UnityEngine;


public class NormalMonster : MonoBehaviour, IDamageable
{
    public event Action<BigNumber> OnDamaged;
    public event Action OnDied;

    private StageManager _stageManager;

    private const LogCategory CurrentCategory = LogCategory.GameLogic;

    public bool IsDead => _stageManager != null && _stageManager.IsDead;
    public BigNumber CurrentHp => _stageManager?.CurrentHp ?? BigNumber.Zero;
    public BigNumber MaxHp => _stageManager?.MaxHp ?? BigNumber.Zero;

    public void Bind(StageManager stageManager)
    {
        _stageManager = stageManager;

        if (_stageManager != null)
        {
            _stageManager.OnDamaged -= HandleDamaged;
            _stageManager.OnDied -= HandleDied;

            _stageManager.OnDamaged += HandleDamaged;
            _stageManager.OnDied += HandleDied;
        }
    }


    public BigNumber ApplyDamage(BigNumber finalDamage)
    {
        if (_stageManager == null) return BigNumber.Zero;
        return _stageManager.ApplyDamage(finalDamage);
    }

    private void HandleDamaged(BigNumber applied)
    {
        this.PrintLog($"몬스터 피격 받음: {BigNumberFormatter.ToString(applied)}", CurrentCategory);
        OnDamaged?.Invoke(applied);
        // TODO: 피격 연출/HP바 갱신
    }

    private void HandleDied()
    {
        this.PrintLog("몬스터 사망", CurrentCategory);
        OnDied?.Invoke();
        // TODO: 사망 연출/풀 반환
    }
}
