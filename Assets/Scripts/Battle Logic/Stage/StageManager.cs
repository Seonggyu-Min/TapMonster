using System;
using UnityEditor.SceneManagement;

public class StageManager : ITargetProvider
{
    private readonly StageProgressService _stageProgressService;
    private readonly StageMaxHpService _stageMaxHpService;
    private MonsterHpService _monsterHpService;
    private ISaveMark _saveMark;
    
    private NormalMonster _enemyView;

    private const LogCategory CurrentCategory = LogCategory.GameLogic;

    public event Action<int> OnStageChanged
    {
        add => _stageProgressService.OnStageChanged += value;
        remove => _stageProgressService.OnStageChanged -= value;
    }
    public event Action<BigNumber> OnDamaged
    {
        add => _monsterHpService.OnDamaged += value;
        remove => _monsterHpService.OnDamaged -= value;
    }
    public event Action OnDied
    {
        add => _monsterHpService.OnDied += value;
        remove => _monsterHpService.OnDied -= value;
    }
    public event Action OnTargetChanged;
    public event Action<int> OnBossStageStarted; // int: stage
    public event Action<int> OnBossStageEnded;   // int: stage


    public StageManager(
        StageProgressService stageProgressService,
        StageMaxHpService stageHpService,
        MonsterHpService monsterHpService
        )
    {
        _stageProgressService = stageProgressService;
        _stageMaxHpService = stageHpService;
        _monsterHpService = monsterHpService;
    }
    public void Initialize(ISaveMark saveMark)
    {
        _saveMark = saveMark;
    }
    public void Activate()
    {
        _monsterHpService.OnDied += HandleMonsterDefeated;
        _monsterHpService.OnDamaged += HandleMonsterDamaged;
    }
    public void Deactivate()
    {
        _monsterHpService.OnDied -= HandleMonsterDefeated;
        _monsterHpService.OnDamaged -= HandleMonsterDamaged;
    }


    public int CurrentStage => _stageProgressService.CurrentStage;
    public IDamageable CurrentTarget => _enemyView;
    public TargetType CurrentTargetType
        => _stageMaxHpService.IsBossStage(CurrentStage) ? TargetType.Boss : TargetType.Normal;


    public BigNumber CurrentHp => _monsterHpService.CurrentHp;
    public BigNumber MaxHp => _monsterHpService.MaxHp;
    public bool HasLoadedValue => _monsterHpService.HasLoadedValue;
    public bool IsDead => _monsterHpService.IsDead;


    public void BindEnemyView(NormalMonster enemyView)
    {
        _enemyView = enemyView;
        if (_enemyView != null)
        {
            _enemyView.Bind(this);
        }

        OnTargetChanged?.Invoke();
    }

    // 스테이지 기준으로 모델 리셋
    public void SpawnOrResetEnemy()
    {
        int currentStage = _stageProgressService.CurrentStage;

        BigNumber maxHp = _stageMaxHpService.GetMonsterMaxHp(currentStage);

        this.PrintLog($"SpawnOrResetEnemy stage={CurrentStage} " +
            $"maxHp=({maxHp.Mantissa}, e{maxHp.Exponent})"
            , CurrentCategory);

        _monsterHpService.InitializeHp(maxHp);
        OnTargetChanged?.Invoke();

        _saveMark.MarkDirty(SaveDirtyFlags.MonsterHp);

        if (_stageMaxHpService.IsBossStage(currentStage))
        {
            OnBossStageStarted?.Invoke(currentStage);
        }
    }

    public BigNumber GetMonsterHpForCurrentStage()
        => _stageMaxHpService.GetMonsterMaxHp(_stageProgressService.CurrentStage);

    public void RollbackFromBossTimeout()
    {
        int currentStage = _stageProgressService.CurrentStage;
        if (!_stageMaxHpService.IsBossStage(currentStage)) return;

        OnBossStageEnded?.Invoke(currentStage);
        _stageProgressService.DecreaseStage();

        _saveMark.MarkDirty(SaveDirtyFlags.Stage);
        _saveMark.MarkDirty(SaveDirtyFlags.MonsterHp);
        _saveMark.RequestSave();

        SpawnOrResetEnemy();
    }

    public void RaiseBossStageStartedIfBoss()
    {
        int stage = _stageProgressService.CurrentStage;
        if (_stageMaxHpService.IsBossStage(stage))
        {
            OnBossStageStarted?.Invoke(stage);
        }
    }


    private void HandleMonsterDefeated()
    {
        int prevStage = _stageProgressService.CurrentStage;
        bool wasBoss = _stageMaxHpService.IsBossStage(prevStage);
        if (wasBoss)
        {
            OnBossStageEnded?.Invoke(prevStage);
        }

        _stageProgressService.AdvanceStage();

        _saveMark.MarkDirty(SaveDirtyFlags.MonsterHp);
        _saveMark.MarkDirty(SaveDirtyFlags.Stage);
        _saveMark.RequestSave();

        SpawnOrResetEnemy();
    }
    private void HandleMonsterDamaged(BigNumber applied)
    {
        _saveMark.MarkDirty(SaveDirtyFlags.MonsterHp);
    }


    public void SetHpSilently(BigNumber maxHp, BigNumber currentHp)
        => _monsterHpService.SetHpSilently(maxHp, currentHp);
    public void InitializeHp(BigNumber maxHp)
        => _monsterHpService.InitializeHp(maxHp);
    public BigNumber ApplyDamage(BigNumber finalDamage)
        => _monsterHpService.ApplyDamage(finalDamage);
    public void SetLoadedFlag(bool loaded)
        => _monsterHpService.SetLoadedFlag(loaded);
}
