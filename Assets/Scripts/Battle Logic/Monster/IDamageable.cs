using System;

public interface IDamageable
{
    bool IsDead { get; }
    BigNumber CurrentHp { get; }
    BigNumber MaxHp { get; }

    event Action<BigNumber> OnDamaged; // 적용된 데미지
    event Action OnDied;

    BigNumber ApplyDamage(BigNumber finalDamage);
}
