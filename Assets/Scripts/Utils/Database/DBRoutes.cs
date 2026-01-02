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
}