/// <summary>
/// DB 경로 접근 시 오타 방지 및 공통화를 위해 사용하는 클래스입니다.
/// </summary>
public static class DBRoutes
{
    // // -- Addressables URL --
    public static string AddressablesCatalogJsonUrl => DatabaseKeys.AddressablesCatalogJsonURL;

    // -- User Data --
    public static string Users => DBPathMaker.Join(DatabaseKeys.Users);
    public static string UsersUid(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid);
    public static string Nickname(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.Nickname);
    public static string NicknameStorage(string nickname) => DBPathMaker.Join(DatabaseKeys.NicknameStorage, nickname);


    // -- Save Data --
    public static string SaveData(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData);
    public static string LastSavedAtUnixMs(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.LastSavedAtUnixMs);

    // - Stage
    public static string Stage(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.Stage);
    public static string CurrentStage(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.Stage, DatabaseKeys.CurrentStage);

    // - Relic
    public static string RelicLevels(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.RelicLevels);
    public static string RelicLevel(string uid, int id) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.RelicLevels, id.ToString());

    // - Upgrade
    public static string UpgradeLevels(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.UpgradeLevels);
    public static string UpgradeLevel(string uid, int id) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.UpgradeLevels, id.ToString());

    // - Skill
    public static string SkillLevels(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.SkillLevels);
    public static string SkillLevel(string uid, int id) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.SkillLevels, id.ToString());

    // - Skill Slot
    public static string SkillSlots(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.SkillSlots);
    public static string SkillSlotsEquipped(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.SkillSlots, DatabaseKeys.Equipped);
    public static string SkillEquippedAt(string uid, int slotIndex) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.SkillSlots, DatabaseKeys.Equipped, slotIndex.ToString());
    public static string SkillSlotsInventory(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.SkillSlots, DatabaseKeys.Inventory);
    public static string SkillInventoryAt(string uid, int slotIndex) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.SkillSlots, DatabaseKeys.Inventory, slotIndex.ToString());

    // - Wallet
    public static string Wallet(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.Wallet);
    public static string Gold(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.Wallet, DatabaseKeys.Gold);
    public static string GoldMantissa(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.Wallet, DatabaseKeys.Gold, DatabaseKeys.Mantissa);
    public static string GoldExponent(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.Wallet, DatabaseKeys.Gold, DatabaseKeys.Exponent);
}