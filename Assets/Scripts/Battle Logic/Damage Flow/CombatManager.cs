using System;
using UnityEngine;

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
        Debug.Log("TryManual triggered.");
        DamageRequest req = new(
            source: DamageSource.Manual,
            skillId: 0,
            skillLevel: 0,
            targetType: _targetProvider.CurrentTargetType
            );

        DamageResult r = _combatService.ResolveHit(
            snapshot: _statManager.GetOrBuildSnapshot(),
            req: req,
            modifiers: _modifiers,
            target: _targetProvider.CurrentTarget
            );

        OnHit?.Invoke(r);
        return r;
    }

    public DamageResult TryAuto()
    {
        DamageRequest req = new DamageRequest(
            source: DamageSource.Auto,
            skillId: 0,
            skillLevel: 0,
            targetType: _targetProvider.CurrentTargetType
            );

        DamageResult r = _combatService.ResolveHit(
            snapshot: _statManager.GetOrBuildSnapshot(),
            req: req,
            modifiers: _modifiers,
            target: _targetProvider.CurrentTarget
            );

        OnHit?.Invoke(r);
        return r;
    }

    public DamageResult TrySkill(int skillId)
    {
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
            skillLevel: _skillManager.GetLevel(skillId),
            targetType: _targetProvider.CurrentTargetType,
            skillMulPerLevel: perLevel,
            canCriticalOverride: true);

        DamageResult r = _combatService.ResolveHit(
            snapshot: _statManager.GetOrBuildSnapshot(),
            req: req,
            modifiers: _modifiers,
            target: _targetProvider.CurrentTarget
            );

        OnHit?.Invoke(r);
        return r;
    }
}
