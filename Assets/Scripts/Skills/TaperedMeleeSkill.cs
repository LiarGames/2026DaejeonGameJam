using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Tapered Melee Skill")]
public class TaperedMeleeSkill : AttackSkill
{
    [SerializeField] private float length = 2f;

    [Range(1f, 179f)]
    [SerializeField] private float tipAngle = 15f;

    public float Length => length;
    public float TipAngle => tipAngle;

    public override void Activate(SkillContext context)
    {
        Vector2 origin = context.Caster.transform.position;
        Vector2 forward = context.Direction.normalized;
        Vector2 right = new Vector2(forward.y, -forward.x);
        float effectiveLength = length * context.Modifiers.RangeMultiplier;

        Collider2D[] targets = Physics2D.OverlapCircleAll(
            origin,
            effectiveLength,
            context.TargetLayer
        );

        foreach (Collider2D target in targets)
        {
            Vector2 displacement =
                (Vector2)target.transform.position - origin;

            float forwardDistance = Vector2.Dot(displacement, forward);

            if (forwardDistance < 0f || forwardDistance > effectiveLength)
                continue;

            float sideDistance = Vector2.Dot(displacement, right);
            float allowedHalfWidth =
                Mathf.Tan(tipAngle * 0.5f * Mathf.Deg2Rad) *
                (effectiveLength - forwardDistance);

            if (Mathf.Abs(sideDistance) <= allowedHalfWidth)
                HitTarget(context, target);
        }

        DrawDebugShape(origin, forward, context);
    }

    private void HitTarget(SkillContext context, Collider2D target)
    {
        float damage = context.AttackPower * damageMultiplier;

        SkillHitProcessor.ApplyHit(
            target,
            damage,
            context.Caster.transform.position,
            context.Modifiers
        );
    }

    private void DrawDebugShape(
        Vector2 origin,
        Vector2 forward,
        SkillContext context,
        float debugDuration = 0.2f
        )
    {   
        Vector2 right = new Vector2(forward.y, -forward.x);
        float rearHalfWidth =
            Mathf.Tan(tipAngle * 0.5f * Mathf.Deg2Rad) * length * context.Modifiers.RangeMultiplier;

        Vector2 rearLeft = origin - right * rearHalfWidth;
        Vector2 rearRight = origin + right * rearHalfWidth;
        Vector2 tip = origin + forward * length * context.Modifiers.RangeMultiplier;

        Debug.DrawLine(rearLeft, rearRight, Color.yellow, debugDuration);
        Debug.DrawLine(rearLeft, tip, Color.yellow, debugDuration);
        Debug.DrawLine(rearRight, tip, Color.yellow, debugDuration);
        Debug.DrawLine(origin, tip, Color.yellow, debugDuration);
    }
}
