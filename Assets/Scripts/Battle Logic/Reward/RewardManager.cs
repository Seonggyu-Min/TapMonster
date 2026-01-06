public class RewardManager
{
    private RewardService _rewardService;
    private GameConfigSO _gameConfigSO;

    public RewardManager(RewardService rewardService, GameConfigSO gameConfigSO)
    {
        _rewardService = rewardService;
        _gameConfigSO = gameConfigSO;
    }
}
