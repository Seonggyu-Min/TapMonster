/// <summary>
/// 최초 스탯 구성 행동을 정의하는 인터페이스
/// </summary>
public interface IStatContributor
{
    void Contribute(ref PlayerStatBuildContext buildContext);
}
