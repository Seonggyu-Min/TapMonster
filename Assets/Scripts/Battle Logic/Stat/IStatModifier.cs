/// <summary>
/// PlayerStatSnapshot 완성 이후 보스 패턴, 액티브 효과, 치명타 등의
/// 최종 데미지 변형의 행동을 정의하는 인터페이스
/// </summary>
public interface IStatModifier
{
    void Modify(ref CalculatingDamageContext damageContext);
}
