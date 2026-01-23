/// <summary>
/// 현재 플레이어 스탯을 스냅샷으로 저장하는 구조체입니다.
/// 이를 통해 특정 시점의 플레이어 스탯을 편리하게 참조할 수 있습니다.
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


public enum DamageSource
{
    Manual,     // 수동 공격
    Auto,       // 자동 공격
    Skill       // 스킬
}
public enum TargetType
{
    None,
    Normal,
    Boss,
}
/// <summary>
/// 데미지 계산 도중 여러 시스템이 값을 수정하기 위한 구조체입니다.
/// 이를 통해 스킬, 유물 등의 효과가 데미지 계산에 반영될 수 있습니다.
/// </summary>
public struct CalculatingDamageContext
{
    public DamageSource DamageSource;

    public BigNumber Damage;

    public bool CanCritical;
    public float CriticalChance;

    public float AdditiveDamagePercent;
    public float DamageMultiplier;

    public int SkillId;

    public TargetType TargetType;
}


/// <summary>
/// 이번 공격 1회의 데미지 요청 정보를 담는 구조체입니다.
/// 이를 통해 데미지 계산에 필요한 기본 정보를 전달할 수 있습니다.
/// </summary>
public readonly struct DamageRequest
{
    public readonly DamageSource Source;
    public readonly int SkillId;
    public readonly int SkillLevel;
    public readonly TargetType TargetType;

    // 스킬 데미지를 어떤 방식으로 만들지(임시)
    // 예: manualFinal 기반 배율
    public readonly float SkillMulPerLevel; // level당 증가치 (예: 0.1 = +10%)
    public readonly bool CanCriticalOverride; // 스킬은 크리 금지 같은 옵션

    public DamageRequest(
        DamageSource source,
        int skillId,
        int skillLevel,
        TargetType targetType,
        float skillMulPerLevel = 0.1f,
        bool canCriticalOverride = true)
    {
        Source = source;
        SkillId = skillId;
        SkillLevel = skillLevel;
        TargetType = targetType;
        SkillMulPerLevel = skillMulPerLevel;
        CanCriticalOverride = canCriticalOverride;
    }
}


/// <summary>
/// 공격 1회의 결과를 담는 구조체입니다.
/// 이를 통해 데미지 결과를 한 번에 전달할 수 있습니다.
/// </summary>
public readonly struct DamageResult
{
    public readonly DamageSource Source;
    public readonly int SkillId;
    public readonly bool IsCritical;
    public readonly BigNumber FinalDamage;
    public readonly bool TargetDied;

    public DamageResult(DamageSource source, int skillId, bool isCritical, BigNumber finalDamage, bool targetDied)
    {
        Source = source;
        SkillId = skillId;
        IsCritical = isCritical;
        FinalDamage = finalDamage;
        TargetDied = targetDied;
    }
}