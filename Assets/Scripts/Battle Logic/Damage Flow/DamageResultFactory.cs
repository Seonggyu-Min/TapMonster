public static class DamageResultFactory
{
    public static DamageResult FailNoTarget(
        DamageSource source,
        int skillId
        )
    {
        return new DamageResult(
            isSuccess: false,
            failReason: DamageFailReason.NoTarget,
            skillFailReason: null,
            source: source,
            skillId: skillId,
            isCritical: false,
            calculatedDamage: BigNumber.Zero,
            appliedDamage: BigNumber.Zero,
            targetDied: false
            );
    }

    public static DamageResult FailSkillBlocked(
        DamageSource source,
        int skillId,
        SkillUseResult reason
        )
    {
        return new DamageResult(
            isSuccess: false,
            failReason: DamageFailReason.SkillBlocked,
            skillFailReason: reason,
            source: source,
            skillId: skillId,
            isCritical: false,
            calculatedDamage: BigNumber.Zero,
            appliedDamage: BigNumber.Zero,
            targetDied: false
            );
    }

    public static DamageResult FailTargetDead(
        DamageSource source,
        int skillId
        )
    {
        return new DamageResult(
            isSuccess: false,
            failReason: DamageFailReason.TargetDead,
            skillFailReason: null,
            source: source,
            skillId: skillId,
            isCritical: false,
            calculatedDamage: BigNumber.Zero,
            appliedDamage: BigNumber.Zero,
            targetDied: true
            );
    }

    public static DamageResult Success(
        DamageSource source,
        int skillId,
        bool isCritical,
        BigNumber calculated,
        BigNumber applied,
        bool targetDied
        )
    {
        return new DamageResult(
            isSuccess: true,
            failReason: DamageFailReason.None,
            skillFailReason: null,
            source: source,
            skillId: skillId,
            isCritical: isCritical,
            calculatedDamage: calculated,
            appliedDamage: applied,
            targetDied: targetDied);
    }
}
