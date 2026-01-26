using UnityEngine;


public class CombatDebugInput : MonoBehaviour
{
    [SerializeField] private int _testSkillId = 20001;

    private GameContext _ctx;

    private const LogCategory CurrentCategory = LogCategory.Debug;


    public void Init(GameContext ctx) => _ctx = ctx;

    private void Update()
    {
        Debug.Log("tick");
        if (_ctx == null)
        {
            Debug.Log("_ctx == null");
            this.PrintLog("_ctx == null", CurrentCategory);
            return;
        }
        Debug.Log("tick _ctx-check OK");

        IDamageable target = _ctx.StageManager.CurrentTarget;
        if (target == null)
        {
            Debug.Log("target == null");
            this.PrintLog("target == null", CurrentCategory);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _ctx.CombatCoordinator.TryManual();
            Debug.Log("Manual attack triggered.");
            this.PrintLog("Manual attack triggered.", CurrentCategory);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _ctx.CombatCoordinator.TrySkill(_testSkillId);
            Debug.Log($"Skill {_testSkillId} used.");
            this.PrintLog($"Skill {_testSkillId} used.", CurrentCategory);
        }
    }
}
