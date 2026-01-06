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
    
    // - Wallet
    public static string Wallet(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.Wallet);
    public static string CurrentGold(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.Wallet, DatabaseKeys.CurrentGold);
    public static string CurrentGoldMantissa(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.Wallet, DatabaseKeys.CurrentGold, DatabaseKeys.Mantissa);
    public static string CurrentGoldExponent(string uid) => DBPathMaker.Join(DatabaseKeys.Users, uid, DatabaseKeys.SaveData, DatabaseKeys.Wallet, DatabaseKeys.CurrentGold, DatabaseKeys.Exponent);
}