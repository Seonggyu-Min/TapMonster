using System;

/// <summary>
/// 최초 스탯 구성 행동을 정의하는 인터페이스
/// </summary>
public interface IStatContributor
{
    /// <summary>
    /// 스탯 재계산용
    /// </summary>
    event Action OnChanged;
    void Contribute(ref PlayerStatBuildContext buildContext);
}
