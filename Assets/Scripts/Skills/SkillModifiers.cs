public struct SkillModifiers
{
    public float KnockbackDistance;
    public float ProjectileCountMultiplier;
    public float CastSpeedMultiplier;

    public static SkillModifiers Default => new SkillModifiers
    {
        KnockbackDistance = 0f,
        ProjectileCountMultiplier = 1f,
        CastSpeedMultiplier = 1f
    };
}
