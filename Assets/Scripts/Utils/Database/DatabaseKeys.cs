/// <summary>
/// 데이터베이스 노드 접근 시 사용하는 키들을 오타 방지를 위해 상수로 관리하는 클래스입니다.
/// </summary>
public static class DatabaseKeys
{
    // -- Addressables URL Keys --
    public const string AddressablesCatalogJsonURL = "AddressablesCatalogJsonURL";

    // -- User Data Keys --
    public const string Users = "Users";
    public const string Nickname = "Nickname";
    public const string NicknameStorage = "NicknameStorage";    // 역인덱스 조회용

    // -- Save Data Keys --
    public const string SaveData = "SaveData";
    public const string LastSavedAtUnixMs = "LastSavedAtUnixMs";

    // - Stage
    public const string Stage = "Stage";
    public const string CurrentStage = "CurrentStage";
    
    // - Relic
    public const string RelicLevels = "RelicLevels";

    // - Upgrade
    public const string UpgradeLevels = "UpgradeLevels";

    // - Skill
    public const string SkillLevels = "SkillLevels";

    // - Skill Slot
    public const string SkillSlots = "SkillSlots";
    public const string Equipped = "Equipped";
    public const string Inventory = "Inventory";

    // - Wallet
    public const string Wallet = "Wallet";
    public const string Gold = "Gold";
    public const string Mantissa = "Mantissa";
    public const string Exponent = "Exponent";

    // - Monster Hp
    public const string MonsterHp = "MonsterHp";
    public const string MaxHp = "MaxHp";
    public const string CurrentHp = "CurrentHp";
    public const string HasValue = "HasValue";

}
