using Cysharp.Threading.Tasks;
using EditorAttributes;
using System.Threading;
using UnityEngine;

public class GameCoordinator : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;

    private GameContext _gameContext;
    private bool _initialized;

    private const LogCategory CurrentCategory = LogCategory.GameLogic;


    public void Init(GameContext gameContext)
    {
        _gameContext = gameContext;

        HandleSubscribe();
        _initialized = true;
    }


    private void OnDestroy()
    {
        HandleUnsubscribe();
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


    private void HandleSubscribe()
    {
        if (!_initialized) return;
        if (_inputManager == null) return;

        _inputManager.OnManualAttack += HandleManualAttack;
    }

    private void HandleUnsubscribe()
    {
        if (_inputManager == null) return;

        _inputManager.OnManualAttack -= HandleManualAttack;
    }

    private void HandleManualAttack()
    {
        this.PrintLog("탭 입력", CurrentCategory);
        PlayerStatSnapshot snap = _gameContext.StatManager.GetOrBuildSnapshot();
        this.PrintLog($"ManualDamage={snap.ManualFinalDamage.Mantissa}e{snap.ManualFinalDamage.Exponent}", CurrentCategory);
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
}
