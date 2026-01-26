using System;

public class PlayerPresenter : IDisposable
{
    private PlayerView _playerView;
    private CombatManager _combatManager;
    private SkillManager _skillManager;
    private ManualAttackConfigSO _manualAttackConfigSO;
    private SkillConfigSO _skillConfigSO;

    private bool _activated;

    public PlayerPresenter(
        PlayerView playerView,
        CombatManager combatManager,
        SkillManager skillManager,
        ManualAttackConfigSO manualAttackConfigSO,
        SkillConfigSO skillConfigSO
        )
    {
        _playerView = playerView;
        _combatManager = combatManager;
        _skillManager = skillManager;
        _manualAttackConfigSO = manualAttackConfigSO;
        _skillConfigSO = skillConfigSO;
    }
    public void Initialize() { /*no op*/ }
    public void Activate()
    {
        if (_activated) return;
        _activated = true;

        _skillManager.OnSkillUsed += HandleSkillAnimation;
        _combatManager.OnHit += HandleManualAnimation;
    }
    public void Dispose()
    {
        if (!_activated) return;
        _activated = false;

        _skillManager.OnSkillUsed -= HandleSkillAnimation;
        _combatManager.OnHit -= HandleManualAnimation;
    }

    private void HandleManualAnimation(DamageResult damageResult)
    {
        if (damageResult.Source != DamageSource.Manual) return;
        var currentKey = _manualAttackConfigSO.AttackAnimationKey;
        _playerView.PlayAnimation(currentKey);
    }

    private void HandleSkillAnimation(SkillUseEvent skillUseEvent)
    {
        _skillConfigSO.TryGet(skillUseEvent.SkillId, out var def);
        var currentKey = def.AnimationKey;
        _playerView.PlayAnimation(currentKey);
    }
}
