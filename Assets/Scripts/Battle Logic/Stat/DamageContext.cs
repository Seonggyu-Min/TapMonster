public enum DamageSource
{
    Manual,     // 수동 공격
    Auto,       // 자동 공격
    Skill       // 스킬
}

public struct DamageContext
{
    public DamageSource DamageSource;
    
    public BigNumber Damage;

    public bool CanCritical;
    public float CriticalChance;

    public float AdditiveMultiplier;
    public float MultiplicativeMultiplier;

    public int SkillId;
}
