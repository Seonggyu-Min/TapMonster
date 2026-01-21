using System;

public class StageHpService
{
    private readonly StageConfigSO _stageConfigSO;

    private IDamageable _currentEnemy;
    public event Action OnMonsterDefeated;
    
    public IDamageable CurrentEnemy => _currentEnemy;


    public StageHpService(StageConfigSO stageConfigSO)
    {
        _stageConfigSO = stageConfigSO;
    }

    public BigNumber GetMonsterHp(int stage)
    {
        // TODO: config 기반 커브 적용
        return BigNumber.One;
    }

    public void BindEnemy(IDamageable enemy)
    {
        if (_currentEnemy != null)
            _currentEnemy.OnDied -= HandleEnemyDied;

        _currentEnemy = enemy;

        if (_currentEnemy != null)
            _currentEnemy.OnDied += HandleEnemyDied;
    }

    public void SpawnOrResetForCurrentStage(int stage)
    {
        if (_currentEnemy == null) return;

        BigNumber hp = GetMonsterHp(stage);
        _currentEnemy.Initialize(hp);
    }

    public void HandleEnemyDied()
    {
        OnMonsterDefeated?.Invoke();
    }
}
