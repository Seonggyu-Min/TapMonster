using System;

public class WalletManager
{
    private readonly ISaveMark _saveMark;
    private WalletService _walletService;

    public event Action<CurrencyId, BigNumber> OnCurrencyChanged;   // UI에서 구독 필요없으면 없애도 될 듯

    public WalletManager(WalletService walletService, ISaveMark saveMark)
    {
        _walletService = walletService;
        _saveMark = saveMark;
    }

    public BigNumber Get(CurrencyId id) => _walletService.Get(id);

    public void Earn(CurrencyId id, BigNumber amount, bool requestSave = true)
    {
        _walletService.Earn(id, amount);
        OnCurrencyChanged?.Invoke(id, _walletService.Get(id));

        if (requestSave)
        {
            _saveMark.MarkDirty(SaveDirtyFlags.Wallet);
            _saveMark.RequestSave();
        }
    }

    public bool CanAfford(CurrencyId id, BigNumber cost) => _walletService.CanAfford(id, cost);
}
