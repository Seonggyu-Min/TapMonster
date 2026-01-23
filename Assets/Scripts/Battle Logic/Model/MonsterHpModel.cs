using System;

public class MonsterHpModel
{
    public event Action<BigNumber> OnDamaged;
    public event Action OnDied;

    public BigNumber CurrentHp { get; private set; }
    public BigNumber MaxHp { get; private set; }
    public bool HasLoadedValue { get; private set; }
    public bool IsDead => CurrentHp <= BigNumber.Zero;

    /// <summary>
    /// 로드 및 동기화용, 이벤트 발생 안함
    ///
    public void SetSilently(BigNumber maxHp, BigNumber currentHp)
    {
        MaxHp = BigNumber.Max(BigNumber.Zero, maxHp);
        CurrentHp = BigNumber.Clamp(currentHp, BigNumber.Zero, MaxHp);
        HasLoadedValue = true;
    }

    /// <summary>
    /// 게임 시작 및 스폰 시, 이벤트 발생 안함
    /// </summary>
    public void Initialize(BigNumber maxHp)
    {
        MaxHp = BigNumber.Max(BigNumber.Zero, maxHp);
        CurrentHp = MaxHp;
        HasLoadedValue = false;
    }

    /// <summary>
    /// 로드 완료 플래그 설정
    /// </summary>
    public void SetLoadedFlag(bool loaded) => HasLoadedValue = loaded;


    /// <summary>
    /// 게임 진행 중 데미지 처리, 이벤트 발생
    /// </summary>
    public BigNumber ApplyDamage(BigNumber finalDamage)
    {
        if (IsDead) return BigNumber.Zero;
        if (finalDamage <= BigNumber.Zero) return BigNumber.Zero;

        BigNumber before = CurrentHp;
        BigNumber after = BigNumber.Max(BigNumber.Zero, before - finalDamage);

        CurrentHp = after;

        BigNumber applied = before - after;
        OnDamaged?.Invoke(applied);

        if (IsDead) OnDied?.Invoke();

        return applied;
    }
}
