using UnityEngine;

public static class DamageFormula
{
    public static BigNumber ComputeManualFinal(ref PlayerStatBuildContext ctx)
    {
        float add = Mathf.Max(0f, ctx.ManualAdditiveDamagePercent);
        float mul = Mathf.Max(0f, ctx.ManualDamageMultiplier);
        float all = Mathf.Max(0f, ctx.FinalAllDamageMultiplier);

        // Base * (1 + add) * mul * all
        BigNumber result = ctx.ManualDamage;
        result *= (1.0 + add);
        result *= mul;
        result *= all;
        return result;
    }

    public static BigNumber ComputeAutoFinal(ref PlayerStatBuildContext ctx)
    {
        float add = Mathf.Max(0f, ctx.AutoAdditiveDamagePercent);
        float mul = Mathf.Max(0f, ctx.AutoDamageMultiplier);
        float all = Mathf.Max(0f, ctx.FinalAllDamageMultiplier);

        BigNumber result = ctx.AutoDamage;
        result *= (1.0 + add);
        result *= mul;
        result *= all;
        return result;
    }
}
