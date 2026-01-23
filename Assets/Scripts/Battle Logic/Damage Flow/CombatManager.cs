using System;

public interface ITargetProvider
{
    IDamageable CurrentTarget { get; }
    TargetType CurrentTargetType { get; }
}


/// <summary>
/// 공격 input의 단일 진입점입니다.
/// </summary>
public class CombatManager
{
    private readonly CombatService _combatService;
    private readonly SkillConfigSO _skillConfigSO;

    private StatManager _statManager;
    private SkillManager _skillManager;    // 스킬 레벨 조회용
    private ITargetProvider _targetProvider;    // 타겟 전달
    private IStatModifier[] _modifiers;

    public event Action<DamageResult> OnHit;

    public CombatManager(CombatService combatService, SkillConfigSO skillConfigSO)
    {
        _combatService = combatService;
        _skillConfigSO = skillConfigSO;
    }
    public void Initialize(
        StatManager statManager,
        SkillManager skillManager,
        ITargetProvider targetProvider,
        IStatModifier[] modifiers
        )
    {
        _statManager = statManager;
        _skillManager = skillManager;
        _targetProvider = targetProvider;
        _modifiers = modifiers;
    }
    public void Activate() { /* no op*/ }
    public void Deactivate() { /* no op*/ }


    public DamageResult TryManual()
    {
        DamageRequest req = new(
            source: DamageSource.Manual,
            skillId: 0,
            skillLevel: 0,
            targetType: _targetProvider.CurrentTargetType
            );

        return TryHitBase(req);
    }

    public DamageResult TryAuto()
    {
        DamageRequest req = new(
            source: DamageSource.Auto,
            skillId: 0,
            skillLevel: 0,
            targetType: _targetProvider.CurrentTargetType
            );
        return TryHitBase(req);
    }

    public DamageResult TrySkill(int skillId)
    {
        // 1. 타겟 확보, 스킬 사용 먼저 해봐야 돼서 여기서 먼저 처리
        IDamageable target = _targetProvider.CurrentTarget;
        if (target == null)
        {
            return DamageResultFactory.FailNoTarget(DamageSource.Skill, skillId);
        }
        if (target.IsDead)
        {
            return DamageResultFactory.FailTargetDead(DamageSource.Skill, skillId);
        }

        // 2. 사용 가능 체크 / 성공 시 쿨타임 시작
        SkillUseResult use = _skillManager.TryUseSkill(skillId);
        if (use != SkillUseResult.Success)
        {
            return DamageResultFactory.FailSkillBlocked(DamageSource.Skill, skillId, use);
        }

        // 3. 스킬 정의 기반으로 DamageRequest 구성
        _skillConfigSO.TryGet(skillId, out var def);

        float perLevel = 0f;
        if (def.Kind == SkillKind.Active)
        {
            perLevel = (float)def.ActiveMulPerLevel;
        }
        else if (def.Kind == SkillKind.Passive)
        {
            perLevel = (float)def.PassiveValuePerLevel;
        }

        DamageRequest req = new(
            source: DamageSource.Skill,
            skillId: skillId,
            skillLevel: _skillManager.GetLevel(skillId),
            targetType: _targetProvider.CurrentTargetType,
            skillMulPerLevel: perLevel,
            canCriticalOverride: true
        );

        return TryHitBase(req, target);
    }


    private DamageResult TryHitBase(DamageRequest req, IDamageable target = null)
    {
        target ??= _targetProvider.CurrentTarget;

        if (target == null)
        {
            return DamageResultFactory.FailNoTarget(req.Source, req.SkillId);
        }
        if (target.IsDead)
        {
            return DamageResultFactory.FailTargetDead(req.Source, req.SkillId);
        }
        
        // 계산
        DamageResultPreview preview = _combatService.ResolveDamageOnly(
            snapshot: _statManager.GetOrBuildSnapshot(),
            req: req,
            modifiers: _modifiers
        );

        // 적용
        BigNumber applied = target.ApplyDamage(preview.CalculatedDamage);
        bool died = target.IsDead;

        // 최종 결과
        DamageResult r = DamageResultFactory.Success(
            req.Source,
            req.SkillId,
            preview.IsCritical,
            preview.CalculatedDamage,
            applied,
            died
        );

        OnHit?.Invoke(r);
        return r;
    }
}
