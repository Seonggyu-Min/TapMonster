public static class PlayerStatBuildContextFactory
{
    public static PlayerStatBuildContext CreateBase(float autoInterval = 0.2f)
    {
        return new PlayerStatBuildContext
        {
            ManualDamage = BigNumber.One,
            AutoDamage = BigNumber.One,

            AutoDamageInterval = autoInterval,

            ManualCriticalChance = 0f,
            AutoCriticalChance = 0f,

            ManualAdditiveDamagePercent = 0f,
            AutoAdditiveDamagePercent = 0f,

            ManualDamageMultiplier = 1f,
            AutoDamageMultiplier = 1f,

            FinalAllDamageMultiplier = 1f,
        };
    }
}
