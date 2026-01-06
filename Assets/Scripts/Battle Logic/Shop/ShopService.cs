public class ShopService
{
    private RelicModel _relicModel;
    private WalletModel _walletModel;


    public ShopService(RelicModel relicModel, WalletModel walletModel)
    {
        _relicModel = relicModel;
        _walletModel = walletModel;
    }
}
