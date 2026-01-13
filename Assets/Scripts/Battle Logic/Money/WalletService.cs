public enum SpendResult
{
    Success,
    NotEnough,
    InvalidAmount
}

public class WalletService
{
    private WalletModel _walletModel;

    public WalletService(WalletModel walletModel)
    {
        _walletModel = walletModel;
    }

    public BigNumber Get(CurrencyId id) => _walletModel.Get(id);

    public void Earn(CurrencyId id, BigNumber amount)
    {
        if (amount.Mantissa <= 0) return; // 0 이하의 수익은 무시
        _walletModel.Set(id, _walletModel.Get(id) + amount);
    }

    public SpendResult TrySpend(CurrencyId id, BigNumber cost)
    {
        if (cost.Mantissa <= 0) return SpendResult.InvalidAmount;

        BigNumber cur = _walletModel.Get(id);
        if (cur < cost) return SpendResult.NotEnough;

        _walletModel.Set(id, cur - cost);
        return SpendResult.Success;
    }

    public bool CanAfford(CurrencyId id, BigNumber cost)
        => cost.Mantissa > 0 && _walletModel.Get(id) >= cost;
}
