public enum PurchaseResult
{
    Success,
    NotEnoughCurrency,
    InvalidCost,
    Blocked,
}

public class PurchaseService
{
    private readonly WalletService _walletService;

    public PurchaseService(WalletService walletService)
    {
        _walletService = walletService;
    }

    public bool CanPay(in Cost cost)
    {
        return _walletService.CanPay(cost.Currency, cost.Amount);
    }

    public PurchaseResult TryPay(in Cost cost)
    {
        if (cost.Amount.Mantissa <= 0) return PurchaseResult.InvalidCost;

        var r = _walletService.TrySpend(cost.Currency, cost.Amount);
        return r == SpendResult.Success ? PurchaseResult.Success : PurchaseResult.NotEnoughCurrency;
    }
}
