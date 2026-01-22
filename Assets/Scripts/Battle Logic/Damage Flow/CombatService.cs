using UnityEngine;

public class CombatService
{
    private readonly float _criticalMultiplier; // 크리 배수(임시: 2.0)

    public CombatService(float criticalMultiplier = 2.0f)
    {
        _criticalMultiplier = Mathf.Max(0f, criticalMultiplier);
    }

    public DamageResult ResolveHit(
        in PlayerStatSnapshot snapshot,
        in DamageRequest req,
        IStatModifier[] modifiers,
        IDamageable target)
    {
        if (target == null || target.IsDead)
            return new DamageResult(req.Source, req.SkillId, false, BigNumber.Zero, true);

        // 1) 기본 DamageContext 생성 (Snapshot 기반)
        CalculatingDamageContext ctx = CreateBaseContext(snapshot, req);

        // 2) Modifier 체인 적용 (스킬, 보스패턴, 버프 등)
        if (modifiers != null)
        {
            for (int i = 0; i < modifiers.Length; i++)
                modifiers[i]?.Modify(ref ctx); // :contentReference[oaicite:5]{index=5}
        }

        // 3) 크리 판정 + 최종 데미지 정산
        bool isCrit = RollCritical(ctx.CanCritical, ctx.CriticalChance);
        BigNumber finalDamage = SettleFinalDamage(ref ctx, isCrit);

        // 4) 타겟 적용
        BigNumber applied = target.ApplyDamage(finalDamage);
        bool died = target.IsDead;

        return new DamageResult(req.Source, req.SkillId, isCrit, applied, died);
    }

    private CalculatingDamageContext CreateBaseContext(in PlayerStatSnapshot snap, in DamageRequest req)
    {
        CalculatingDamageContext ctx = new();
        ctx.DamageSource = req.Source;
        ctx.TargetType = req.TargetType;

        ctx.SkillId = req.SkillId;

        ctx.AdditiveDamagePercent = 0f;
        ctx.DamageMultiplier = 1f;

        // 기본 크리 가능
        ctx.CanCritical = req.CanCriticalOverride;

        // Source별 기본 데미지/크리확률 세팅
        switch (req.Source)
        {
            case DamageSource.Manual:
                ctx.Damage = snap.ManualFinalDamage;
                ctx.CriticalChance = snap.ManualCriticalChance;
                break;

            case DamageSource.Auto:
                ctx.Damage = snap.AutoFinalDamage;
                ctx.CriticalChance = snap.AutoCriticalChance;
                break;

            case DamageSource.Skill:
            default:
                // 스킬 데미지: manualFinal 기반 배율(임시 공식)
                int lv = Mathf.Max(0, req.SkillLevel);
                float mul = 1f + (Mathf.Max(0f, req.SkillMulPerLevel) * lv);

                ctx.Damage = snap.ManualFinalDamage * mul;
                ctx.CriticalChance = snap.ManualCriticalChance; // 원하면 스킬용 따로 분리 가능
                break;
        }

        return ctx;
    }

    private bool RollCritical(bool canCritical, float chance)
    {
        if (!canCritical) return false;

        float c = Mathf.Clamp01(chance);
        if (c <= 0f) return false;

        return Random.value < c;
    }

    private BigNumber SettleFinalDamage(ref CalculatingDamageContext ctx, bool isCritical)
    {
        // ctx.Damage는 이미 snapshot 기반 기본값이고,
        // modifiers는 AdditiveDamagePercent / DamageMultiplier 등을 건드릴 수 있음.
        float add = Mathf.Max(0f, ctx.AdditiveDamagePercent);
        float mul = Mathf.Max(0f, ctx.DamageMultiplier);

        if (isCritical)
            mul *= _criticalMultiplier;

        BigNumber result = ctx.Damage;
        result *= (1.0 + add);
        result *= mul;
        return result;
    }
}
