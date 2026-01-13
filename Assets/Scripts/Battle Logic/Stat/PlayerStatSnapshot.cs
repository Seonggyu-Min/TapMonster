/// <summary>
/// PlayerStatSnapshot에서 모든 정보를 총합하여 계산된 플레이어 스탯의 결과물 스냅샷입니다.
/// 해당 정보를 통해 보스 패턴, 치명타 계산 등을 Modifier에서 수행하여 최종 데미지를 계산합니다.
/// </summary>
public readonly struct PlayerStatSnapshot
{
    public readonly BigNumber ManualFinalDamage;
    public readonly BigNumber AutoFinalDamage;

    public readonly float ManualCriticalChance;
    public readonly float AutoCriticalChance;

    public readonly float AutoDamageInterval;

    public PlayerStatSnapshot(
        BigNumber manualFinal,
        BigNumber autoFinal,
        float manualCrit,
        float autoCrit,
        float autoInterval)
    {
        ManualFinalDamage = manualFinal;
        AutoFinalDamage = autoFinal;
        ManualCriticalChance = manualCrit;
        AutoCriticalChance = autoCrit;
        AutoDamageInterval = autoInterval;
    }
}
