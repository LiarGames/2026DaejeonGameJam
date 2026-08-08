using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Support/Multi Projectile Support")]
public class MultiProjectileSupport : SupportSkill
{
    [SerializeField] private float projectileMultiplier = 2f;

    public override void ApplyModifier(ref SkillModifiers modifiers)
    {
        modifiers.ProjectileCountMultiplier *= projectileMultiplier;
    }
}
