/// <summary>
/// PlayerStatSnapshot에서 모든 정보를 총합하여 계산된 플레이어 스탯의 결과물 스냅샷입니다.
/// 해당 정보를 통해 보스 패턴, 치명타 계산 등을 Modifier에서 수행하여 최종 데미지를 계산합니다.
/// </summary>
public readonly struct PlayerStatSnapshot
{
    public readonly BigNumber ManualDamage;                             // 자동 공격 데미지
    public readonly BigNumber AutoDamage;                               // 수동 공격 데미지

    public readonly float AutoDamageInterval;                           // 자동 공격 데미지 주기 (일단 고정으로 쓸 것 같음)

    public readonly float ManualCriticalChance;                         // 수동 공격 치명타 확률 (합연산만 가능)
    public readonly float AutoCriticalChance;                           // 자동 공격 치명타 확률 (합연산만 가능)

    public readonly float ManualAdditiveDamageMultiplier;               // 수동 공격 합연산
    public readonly float AutoAdditiveDamageMultiplier;                 // 자동 공격 합연산

    public readonly float ManualMultiplicativeDamageMultiplier;         // 수동 공격 곱연산
    public readonly float AutoMultiplicativeDamageMultiplier;           // 자동 공격 곱연산


    public PlayerStatSnapshot(
        BigNumber manualDamage,
        BigNumber autoDamage,
        float autoDamageInterval,
        float manualCriticalChance,
        float autoCriticalChance,
        float manualAdditiveDamageMultiplier,
        float autoAdditiveDamageMultiplier,
        float manualMultiplicativeDamageMultiplier,
        float autoMultiplicativeDamageMultiplier)
    {
        ManualDamage = manualDamage;
        AutoDamage = autoDamage;
        AutoDamageInterval = autoDamageInterval;
        ManualCriticalChance = manualCriticalChance;
        AutoCriticalChance = autoCriticalChance;
        ManualAdditiveDamageMultiplier = manualAdditiveDamageMultiplier;
        AutoAdditiveDamageMultiplier = autoAdditiveDamageMultiplier;
        ManualMultiplicativeDamageMultiplier = manualMultiplicativeDamageMultiplier;
        AutoMultiplicativeDamageMultiplier = autoMultiplicativeDamageMultiplier;
    }
}
