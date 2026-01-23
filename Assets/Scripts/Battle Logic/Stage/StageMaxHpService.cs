using System;

public class StageMaxHpService
{
    private readonly StageConfigSO _stageConfigSO;

    private IDamageable _currentEnemy;
    public event Action OnMonsterDefeated;

    public IDamageable CurrentEnemy => _currentEnemy;


    public StageMaxHpService(StageConfigSO stageConfigSO)
    {
        _stageConfigSO = stageConfigSO;
    }

    public BigNumber GetMonsterMaxHp(int stage) => _stageConfigSO.GetMaxHp(stage);
    public bool IsBossStage(int stage) => _stageConfigSO.IsBossStage(stage);
}
