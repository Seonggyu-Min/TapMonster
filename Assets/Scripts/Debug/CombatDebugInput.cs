using UnityEngine;


public class CombatDebugInput : MonoBehaviour
{
    [SerializeField] private int _testSkillId = 20001;

    private GameContext _ctx;

    private const LogCategory CurrentCategory = LogCategory.Debug;


    public void Init(GameContext ctx) => _ctx = ctx;

    private void Update()
    {
        if (_ctx == null) return;

        IDamageable target = _ctx.StageManager.CurrentEnemy;
        if (target == null) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _ctx.CombatManager.TryManual(target, TargetType.Normal);
            this.PrintLog("Manual attack triggered.", CurrentCategory);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _ctx.CombatManager.TrySkill(target, _testSkillId, TargetType.Normal);
            this.PrintLog($"Skill {_testSkillId} used.", CurrentCategory);
        }
    }
}
