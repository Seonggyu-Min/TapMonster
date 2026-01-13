public class PurchaseManager
{
    private readonly PurchaseService _purchaseService;
    private readonly ISaveMark _saveMark;

    public PurchaseManager(PurchaseService PurchaseService, ISaveMark saveMark)
    {
        _purchaseService = PurchaseService;
        _saveMark = saveMark;
    }

    public bool CanPay(in Cost cost) => _purchaseService.CanPay(cost);

    public PurchaseResult TryPay(in Cost cost)
    {
        PurchaseResult result = _purchaseService.TryPay(cost);
        if (result == PurchaseResult.Success)
        {
            _saveMark.MarkDirty(SaveDirtyFlags.Wallet);
            _saveMark.RequestSave();
        }
        return result;
    }
}
