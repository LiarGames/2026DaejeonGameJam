using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Melee Skill")]
public class MeleeSkill : AttackSkill
{
    [Header("Attack Shape")]
    [SerializeField] private float radius = 2f;

    [Range(0f, 360f)]
    [SerializeField] private float angle = 90f;

    public override void Activate(SkillContext context)
    {   
        Vector2 origin = GetCastPosition(context);

        PerformAttack(context, origin);

        float effectiveRadius =
            radius * context.Modifiers.RangeMultiplier;

        SpawnVFX(
            origin,
            context.Direction,
            null,
            Vector3.one * effectiveRadius
        );

        DrawDebugSector(
            origin,
            context.Direction,
            radius * context.Modifiers.RangeMultiplier,
            angle
        );
    }

    private void PerformAttack(SkillContext context, Vector2 origin)
    {   
        float effectiveRadius = radius * context.Modifiers.RangeMultiplier;

        Collider2D[] targets = Physics2D.OverlapCircleAll(
            origin,
            effectiveRadius,
            context.TargetLayer
        );

        foreach (Collider2D target in targets)
        {
            Vector2 directionToTarget =
                ((Vector2)target.transform.position -
                 origin).normalized;

            float targetAngle =
                Vector2.Angle(context.Direction, directionToTarget);

            if (targetAngle <= angle / 2f)
            {
                HitTarget(context, target, origin);
            }
        }
    }

    private void HitTarget(
        SkillContext context,
        Collider2D target,
        Vector2 origin)
    {
        float damage = context.AttackPower * damageMultiplier;

        SkillHitProcessor.ApplyHit(
            target,
            damage,
            origin,
            context.Modifiers
        );
    }

    private void DrawDebugSector(
        Vector2 origin,
        Vector2 direction,
        float radius,
        float angle,
        float duration = 0.2f)
    {
        int segments = 20;

        float centerAngle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float startAngle = centerAngle - angle / 2f;
        float step = angle / segments;

        Vector2 previousPoint = origin;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle =
                (startAngle + step * i) * Mathf.Deg2Rad;

            Vector2 point = origin + new Vector2(
                Mathf.Cos(currentAngle),
                Mathf.Sin(currentAngle)
            ) * radius;

            if (i == 0)
                Debug.DrawLine(origin, point, Color.white, duration);
            else
                Debug.DrawLine(previousPoint, point, Color.white, duration);

            previousPoint = point;
        }

        Debug.DrawLine(origin, previousPoint, Color.white, duration);
    }
}
