using System;

public class WalletManager
{
    private ISaveMark _saveMark;
    private WalletService _walletService;

    public event Action<CurrencyId, BigNumber> OnCurrencyChanged
    {
        add => _walletService.OnCurrencyChanged += value;
        remove => _walletService.OnCurrencyChanged -= value;
    }

    public WalletManager(WalletService walletService)
    {
        _walletService = walletService;
    }
    public void Initialize(ISaveMark saveMark)
    {
        _saveMark = saveMark;
    }
    public void Activate() { /* no op*/ }
    public void Deactivate() { /* no op*/ }

    public BigNumber Get(CurrencyId id) => _walletService.Get(id);

    public void Earn(CurrencyId id, BigNumber amount, bool requestSave = true)
    {
        _walletService.Earn(id, amount);

        if (requestSave)
        {
            _saveMark.MarkDirty(SaveDirtyFlags.Wallet);
            _saveMark.RequestSave();
        }
    }

    public bool CanAfford(CurrencyId id, BigNumber cost) => _walletService.CanPay(id, cost);
}
