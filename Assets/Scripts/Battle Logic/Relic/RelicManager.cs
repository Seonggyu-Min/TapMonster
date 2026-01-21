public class RelicManager : IStatContributor
{
    private readonly RelicService _relicService;
    private readonly RelicGachaService _relicGachaService;
    private readonly GameConfigSO _gameConfigSO;
    private PurchaseManager _purchaseManager;
    private ISaveMark _saveMark;

    public RelicManager(
        RelicService relicService,
        RelicGachaService relicGachaService,
        GameConfigSO gameConfigSO
        )
    {
        _relicService = relicService;
        _relicGachaService = relicGachaService;
        _gameConfigSO = gameConfigSO;
    }
    public void Initialize(ISaveMark saveMark, PurchaseManager purchaseManager)
    {
        _saveMark = saveMark;
        _purchaseManager = purchaseManager;
    }
    public void Activate() { /* no op*/ }
    public void Deactivate() { /* no op*/ }


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
