using UnityEngine;

public static class SkillHitProcessor
{
    public static void ApplyHit(
        EnemyHealth enemy,
        float damage,
        Vector2 hitOrigin,
        SkillModifiers modifiers)
    {
        if (enemy == null)
            return;

        enemy.TakeDamage(damage);

        if (modifiers.KnockbackDistance > 0f)
        {
            enemy.ApplyKnockback(
                hitOrigin,
                modifiers.KnockbackDistance
            );
        }
    }
}
