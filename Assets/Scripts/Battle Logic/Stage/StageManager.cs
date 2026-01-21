using System;

public class StageManager
{
    private readonly StageProgressService _stageProgressService;
    private readonly StageHpService _stageHpService;

    private ISaveMark _saveMark;

    public event Action<int> OnStageChanged;

    public StageManager(
        StageProgressService stageProgressService,
        StageHpService stageHpService
        )
    {
        _stageProgressService = stageProgressService;
        _stageHpService = stageHpService;
    }
    public void Initialize(ISaveMark saveMark)
    {
        _saveMark = saveMark;
    }
    public void Activate()
    {
        _stageHpService.OnMonsterDefeated += HandleMonsterDefeated;
    }
    public void Deactivate()
    {
        if (_stageHpService != null)
        {
            _stageHpService.OnMonsterDefeated -= HandleMonsterDefeated;
        }
    }



    public int CurrentStage => _stageProgressService.CurrentStage;

    public IDamageable CurrentEnemy => _stageHpService.CurrentEnemy;

    public void BindEnemy(IDamageable enemy) => _stageHpService.BindEnemy(enemy);

    public void SpawnOrResetEnemy() => _stageHpService.SpawnOrResetForCurrentStage(_stageProgressService.CurrentStage);

    public void OnMonsterDefeated()
    {
        _stageProgressService.AdvanceStage();

        _saveMark.MarkDirty(SaveDirtyFlags.Stage);
        _saveMark.RequestSave();

        OnStageChanged?.Invoke(_stageProgressService.CurrentStage);
    }

    public BigNumber GetMonsterHpForCurrentStage()
        => _stageHpService.GetMonsterHp(_stageProgressService.CurrentStage);


    private void HandleMonsterDefeated()
    {
        OnMonsterDefeated(); // 스테이지 진행/저장/이벤트
        SpawnOrResetEnemy(); // 다음 스테이지 몬스터 스폰
    }
}
