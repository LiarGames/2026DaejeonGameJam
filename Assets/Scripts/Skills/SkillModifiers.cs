public struct SkillModifiers
{
    // General skill modifiers
    public float CastSpeedMultiplier;
    public float RangeMultiplier;

    // Projectile modifiers
    public float ProjectileCountMultiplier;

    // On-hit modifiers
    public float KnockbackDistance;


    public static SkillModifiers Default => new SkillModifiers
    {
        KnockbackDistance = 0f,
        ProjectileCountMultiplier = 1f,
        CastSpeedMultiplier = 1f,
        RangeMultiplier = 1f
    };
}
