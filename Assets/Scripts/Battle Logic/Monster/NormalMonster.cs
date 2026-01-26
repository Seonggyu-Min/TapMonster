using System;
using UnityEngine;


public class NormalMonster : MonoBehaviour, IDamageable
{
    [SerializeField] private Animator _animator;

    public event Action<BigNumber> OnDamaged;
    public event Action OnDied;

    private StageManager _stageManager;

    private const LogCategory CurrentCategory = LogCategory.GameLogic;

    public bool IsDead => _stageManager != null && _stageManager.IsDead;
    public BigNumber CurrentHp => _stageManager?.CurrentHp ?? BigNumber.Zero;
    public BigNumber MaxHp => _stageManager?.MaxHp ?? BigNumber.Zero;


    private void Awake()
    {
        this.TryBindComponent(ref _animator, CurrentCategory);
    }

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

        PlayAnimation(AnimationKey.Enemy_Hit);
        // TODO: Hit 애니메이션 Manual Damage와 Auto Damage 분리?
    }

    private void HandleDied()
    {
        this.PrintLog("몬스터 사망", CurrentCategory);
        OnDied?.Invoke();

        PlayAnimation(AnimationKey.Enemy_Die);
        // TODO: 풀 반환
    }

    private void PlayAnimation(AnimationKey key)
    {
        int hash = AnimationHashes.Get(key);
        if (hash == 0)
        {
            this.PrintLog($"애니메이션을 찾을 수 없습니다 key: {key}", CurrentCategory, LogType.Warning);
            return;
        }

        _animator.Play(hash, 0, 0f);
    }
}
