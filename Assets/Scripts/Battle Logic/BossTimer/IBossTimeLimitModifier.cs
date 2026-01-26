public interface IBossTimeLimitModifier
{
    /// <summary>
    /// baseSeconds에 유물, 스킬 등을 참조하여 baseSeconds에 더해 전달
    /// </summary>
    float Modify(float baseSeconds);
}
