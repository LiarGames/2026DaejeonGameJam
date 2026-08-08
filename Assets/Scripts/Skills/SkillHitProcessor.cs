using UnityEngine;

public static class SkillHitProcessor
{
    public static void ApplyHit(
        Collider2D target,
        float damage,
        Vector2 hitOrigin,
        SkillModifiers modifiers)
    {
        IDamageable damageable =
            target.GetComponentInParent<IDamageable>();

        if (damageable == null)
            return;

        damageable.TakeDamage(damage);

        if (modifiers.KnockbackDistance > 0f)
        {
            IKnockbackable knockbackable =
                target.GetComponentInParent<IKnockbackable>();

            knockbackable?.ApplyKnockback(
                hitOrigin,
                modifiers.KnockbackDistance
            );
        }
    }
}
