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

    // -- Friend Data Keys --

}
