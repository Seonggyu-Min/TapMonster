using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class GameCoordinator : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private MonsterSpawner _monsterSpawner;

    private GameStateModel _gameStateModel;
    private GameContext _gameContext;
    private bool _initialized;

    private const LogCategory CurrentCategory = LogCategory.GameLogic;


    public void Init(GameStateModel gameStateModel, GameContext gameContext)
    {
        _gameStateModel = gameStateModel;
        _gameContext = gameContext;
        _initialized = true;
    }

    public void Activate()
    {
        ApplyLoadedBossTimer();     // 보스 타이머 DTO -> Model 적용
        SpawnAndApplyLoadedHp();    // 최초 스폰 및 체력 로드
        _gameContext.StageManager   // 보스면 타이머 표기용 이벤트 발생
            .RaiseBossStageStartedIfBoss();
    }

    private void Update()
    {
        _gameContext.BossTimerCoordinator.Tick(Time.deltaTime);
    }

    private void OnDestroy()
    {
        
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause) return;
        if (!_initialized) return;

        ForceSaveOnExitAsync().Forget();
    }
    private void OnApplicationQuit()
    {
        if (!_initialized) return;
        ForceSaveOnExitAsync().Forget();
    }


    private async UniTaskVoid ForceSaveOnExitAsync()
    {
        CancellationToken ct = this.GetCancellationTokenOnDestroy();
        await _gameContext.SaveLoadManager.ForceSaveAsync(ct);
    }

    private void ApplyLoadedBossTimer()
    {
        BossTimerDTO bt = _gameStateModel.LoadedBossTimerDTO;

        if (bt != null)
        {
            _gameContext.BossTimerCoordinator.ApplyLoadedState(
                bt.IsRunning,
                bt.BossStage,
                bt.RemainingSeconds
            );
        }
    }


    private void SpawnAndApplyLoadedHp()
    {
        bool hasHp = _gameContext.StageManager.HasLoadedValue;
        _monsterSpawner.Spawn(_gameContext.StageManager, resetHp: !hasHp);
    }
}
