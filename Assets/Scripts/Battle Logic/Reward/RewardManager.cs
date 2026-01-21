public class RewardManager
{
    private readonly RewardService _rewardService;
    private readonly GameConfigSO _gameConfigSO;

    public RewardManager(RewardService rewardService, GameConfigSO gameConfigSO)
    {
        _rewardService = rewardService;
        _gameConfigSO = gameConfigSO;
    }
    public void Initialize() { /* no op*/ }
    public void Activate() { /* no op*/ }
    public void Deactivate() { /* no op*/ }
}
