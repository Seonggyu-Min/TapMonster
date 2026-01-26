public class GameContext
{
    public GameStateModel GameStateModel { get; init; }
    public SaveLoadManager SaveLoadManager { get; init; }
    public StageManager StageManager { get; init; }
    public RelicManager RelicManager { get; init; }
    public UpgradeManager UpgradeManager { get; init; }
    public SkillManager SkillManager { get; init; }
    public WalletManager WalletManager { get; init; }
    public RewardManager RewardManager { get; init; }
    public PurchaseManager PurchaseManager { get; init; }
    public StatManager StatManager { get; init; }
    public CombatCoordinator CombatCoordinator { get; init; }
}
