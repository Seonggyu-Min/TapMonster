using Cysharp.Threading.Tasks;
using EditorAttributes;
using System.Threading;
using UnityEngine;

public class GameCoordinator : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private MonsterSpawner _monsterSpawner;

    private GameContext _gameContext;
    private bool _initialized;

    private const LogCategory CurrentCategory = LogCategory.GameLogic;


    public void Init(GameContext gameContext)
    {
        _gameContext = gameContext;
        _initialized = true;
    }

    public void Activate()
    {
        SpawnAndApplyLoadedHp();
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


#if UNITY_EDITOR
    [Button("테스트용 업그레이드")]
    public void TestUpgradeClicked()
    {
        bool ok = _gameContext.UpgradeManager.TryUpgrade(10001);
        if (!ok) return;

        // 스탯 재계산
        _gameContext.StatManager.MarkDirty();

        // 자동 저장
        _gameContext.SaveLoadManager.MarkDirty(SaveDirtyFlags.Upgrade | SaveDirtyFlags.Wallet);
        _gameContext.SaveLoadManager.RequestSave();
    }


    [Button("테스트용 돈 얻기")]
    public void TestEarnGold()
    {
        BigNumber before = _gameContext.WalletManager.Get(CurrencyId.Gold);
        this.PrintLog($"얻기 전 - Mantissa: {before.Mantissa}, Exponent: {before.Exponent}", CurrentCategory);
        _gameContext.WalletManager.Earn(CurrencyId.Gold, new BigNumber(1, 10));
        BigNumber after = _gameContext.WalletManager.Get(CurrencyId.Gold);
        this.PrintLog($"얻은 후 - Mantissa: {after.Mantissa}, Exponent: {after.Exponent}", CurrentCategory);
    }
#endif

    private async UniTaskVoid ForceSaveOnExitAsync()
    {
        CancellationToken ct = this.GetCancellationTokenOnDestroy();
        await _gameContext.SaveLoadManager.ForceSaveAsync(ct);
    }

    private void SpawnAndApplyLoadedHp()
    {
        bool hasHp = _gameContext.StageManager.HasLoadedValue;
        _monsterSpawner.Spawn(_gameContext.StageManager, resetHp: !hasHp);
    }
}
