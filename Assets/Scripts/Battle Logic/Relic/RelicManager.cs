public class RelicManager : IStatContributor
{
    private RelicService _relicService;
    private RelicGachaService _relicGachaService;
    private GameConfigSO _gameConfigSO;
    private PurchaseManager _purchaseManager;
    private ISaveMark _saveMark;

    public RelicManager(
        RelicService relicService,
        RelicGachaService relicGachaService,
        GameConfigSO gameConfigSO,
        PurchaseManager purchaseManager,
        ISaveMark saveMark
        )
    {
        _relicService = relicService;
        _relicGachaService = relicGachaService;
        _gameConfigSO = gameConfigSO;
        _purchaseManager = purchaseManager;
        _saveMark = saveMark;
    }



    public void TryGachaOnce()
    {
        BigNumber cost = _relicGachaService.GetCurrentRelicGachaCost();

        var result = _purchaseManager.TryPay(new Cost(CurrencyId.Gold, cost));
        if (result != PurchaseResult.Success) return;

        int relicId = _relicGachaService.RollRelicId(_gameConfigSO);

        // 중복이면 레벨업
        // TODO: 풀 레벨 업 이면 다른 처리
        _relicService.AddLevel(relicId, +1);

        _relicService.IncrementRelicGachaCount();

        _saveMark.MarkDirty(SaveDirtyFlags.Relic);
        _saveMark.RequestSave();
    }

    public void Contribute(ref PlayerStatBuildContext buildContext)
    {
        _relicService.ApplyToStat(ref buildContext, _gameConfigSO);
    }
}
