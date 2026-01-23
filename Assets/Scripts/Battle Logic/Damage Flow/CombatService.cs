using UnityEngine;

public class CombatService
{
    private readonly float _criticalMultiplier; // 크리티컬 데미지 배수: 임시로 2.0f

    public CombatService(float criticalMultiplier = 2.0f)
    {
        _criticalMultiplier = Mathf.Max(0f, criticalMultiplier);
    }


    public DamageResultPreview ResolveDamageOnly(
        PlayerStatSnapshot snapshot,
        DamageRequest req,
        IStatModifier[] modifiers
        )
    {
        // 1. Snapshot 기반 DamageContext 생성
        CalculatingDamageContext ctx = CreateBaseContext(snapshot, req);

        // 2. Modifier 순회 적용
        if (modifiers != null)
        {
            for (int i = 0; i < modifiers.Length; i++)
            {
                modifiers[i]?.Modify(ref ctx);
            }
        }

        // 3. 크리티컬 판정
        bool isCrit = RollCritical(ctx.CanCritical, ctx.CriticalChance);

        // 4. 최종 데미지 정산
        BigNumber calculated = SettleFinalDamage(ref ctx, isCrit);

        return new DamageResultPreview(isCrit, calculated);
    }

    private CalculatingDamageContext CreateBaseContext(in PlayerStatSnapshot snap, in DamageRequest req)
    {
        CalculatingDamageContext ctx = new();
        ctx.DamageSource = req.Source;
        ctx.TargetType = req.TargetType;

        ctx.SkillId = req.SkillId;

        ctx.AdditiveDamagePercent = 0f;
        ctx.DamageMultiplier = 1f;

        // 기본 크리 가능 여부
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
                // 스킬 데미지: manualFinal 기반 배율
                // TODO: 배율 설정 config
                int lv = Mathf.Max(0, req.SkillLevel);
                float mul = 1f + (Mathf.Max(0f, req.SkillMulPerLevel) * lv);

                ctx.Damage = snap.ManualFinalDamage * mul;
                ctx.CriticalChance = snap.ManualCriticalChance; // TODO: 스킬용 따로 분리
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
