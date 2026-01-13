public class StatBuilderService
{
    public PlayerStatSnapshot BuildSnapshot(
        GameConfigSO config,
        IStatContributor[] contributors
        )
    {
        PlayerStatBuildContext ctx = PlayerStatBuildContextFactory.CreateBase();

        // contributors가 ctx에 누적
        if (contributors != null)
        {
            for (int i = 0; i < contributors.Length; i++)
            {
                contributors[i]?.Contribute(ref ctx);
            }
        }

        // 최종 계산
        BigNumber manualFinal = DamageFormula.ComputeManualFinal(ref ctx);
        BigNumber autoFinal = DamageFormula.ComputeAutoFinal(ref ctx);

        return new PlayerStatSnapshot(
            manualFinal,
            autoFinal,
            ctx.ManualCriticalChance,
            ctx.AutoCriticalChance,
            ctx.AutoDamageInterval);
    }
}
