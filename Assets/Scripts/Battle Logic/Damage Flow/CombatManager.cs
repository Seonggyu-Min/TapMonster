using System;

public interface ITargetProvider
{
    IDamageable CurrentTarget { get; }
    TargetType CurrentTargetType { get; }
}


public class CombatManager
{
    private readonly CombatService _combatService;
    private readonly SkillConfigSO _skillConfigSO;

    private StatManager _statManager;
    private SkillManager _skillManager;    // 스킬 레벨 조회용
    private StageManager _stageManager;    // 필요할지는 모르겠는데 타겟 타입 조회해야될 수도?
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
        StageManager stageManager,
        IStatModifier[] modifiers
        )
    {
        _statManager = statManager;
        _skillManager = skillManager;
        _stageManager = stageManager;
        _modifiers = modifiers;
    }
    public void Activate() { /* no op*/ }
    public void Deactivate() { /* no op*/ }


    public DamageResult TryManual(IDamageable target, TargetType targetType = TargetType.Normal)
    {
        PlayerStatSnapshot snap = _statManager.GetOrBuildSnapshot();
        DamageRequest req = new DamageRequest(DamageSource.Manual, 0, 0, targetType);

        DamageResult r = _combatService.ResolveHit(snap, req, _modifiers, target);
        OnHit?.Invoke(r);
        return r;
    }

    public DamageResult TryAuto(IDamageable target, TargetType targetType = TargetType.Normal)
    {
        PlayerStatSnapshot snap = _statManager.GetOrBuildSnapshot();
        DamageRequest req = new DamageRequest(DamageSource.Auto, 0, 0, targetType);

        DamageResult r = _combatService.ResolveHit(snap, req, _modifiers, target);
        OnHit?.Invoke(r);
        return r;
    }

    public DamageResult TrySkill(IDamageable target, int skillId, TargetType targetType = TargetType.Normal)
    {
        PlayerStatSnapshot snap = _statManager.GetOrBuildSnapshot();

        int level = _skillManager.GetLevel(skillId);

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

        DamageRequest req = new DamageRequest(
            source: DamageSource.Skill,
            skillId: skillId,
            skillLevel: level,
            targetType: targetType,
            skillMulPerLevel: perLevel,
            canCriticalOverride: true);

        DamageResult r = _combatService.ResolveHit(snap, req, _modifiers, target);
        OnHit?.Invoke(r);
        return r;
    }
}
