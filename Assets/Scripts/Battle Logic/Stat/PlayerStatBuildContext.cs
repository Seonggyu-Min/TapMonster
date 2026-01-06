/// <summary>
/// PlayerStatSnapshot에 전달할 정보를 업그레이드, 유물 등에서 반영하는 과정에서 최초의 정보 구성용 구조체입니다.
/// </summary>
public struct PlayerStatBuildContext
{
    public BigNumber ManualDamage;                              // 자동 공격 데미지
    public BigNumber AutoDamage;                                // 수동 공격 데미지

    public float AutoDamageInterval;                            // 자동 공격 데미지 주기 (일단 고정으로 쓸 것 같음)

    public float ManualCriticalChance;                          // 수동 공격 치명타 확률 (합연산만 가능)
    public float AutoCriticalChance;                            // 자동 공격 치명타 확률 (합연산만 가능)

    public float ManualAdditiveDamageMultiplier;                // 수동 공격 합연산
    public float AutoAdditiveDamageMultiplier;                  // 자동 공격 합연산

    public float ManualMultiplicativeDamageMultiplier;          // 수동 공격 곱연산
    public float AutoMultiplicativeDamageMultiplier;            // 자동 공격 곱연산
}
