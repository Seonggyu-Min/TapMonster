public class PurchaseManager
{
    private readonly PurchaseService _purchaseService;
    private ISaveMark _saveMark;

    public PurchaseManager(PurchaseService PurchaseService)
    {
        _purchaseService = PurchaseService;
        
    }

    public void Initialize(ISaveMark saveMark)
    {
        _saveMark = saveMark;
    }

    public void Activate() { /* no op*/ }
    public void Deactivate() { /* no op*/ }


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
