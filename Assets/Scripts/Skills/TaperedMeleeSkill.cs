using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Tapered Melee Skill")]
public class TaperedMeleeSkill : Skill
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

        Collider2D[] targets = Physics2D.OverlapCircleAll(
            origin,
            length,
            context.EnemyLayer
        );

        foreach (Collider2D target in targets)
        {
            Vector2 displacement =
                (Vector2)target.transform.position - origin;

            float forwardDistance = Vector2.Dot(displacement, forward);

            if (forwardDistance < 0f || forwardDistance > length)
                continue;

            float sideDistance = Vector2.Dot(displacement, right);
            float allowedHalfWidth =
                Mathf.Tan(tipAngle * 0.5f * Mathf.Deg2Rad) *
                (length - forwardDistance);

            if (Mathf.Abs(sideDistance) <= allowedHalfWidth)
                HitTarget(context, target);
        }

        DrawDebugShape(origin, forward);
    }

    private void HitTarget(SkillContext context, Collider2D target)
    {
        float damage = context.Stats.Attack * damageMultiplier;

        target.SendMessage(
            "TakeDamage",
            damage,
            SendMessageOptions.DontRequireReceiver
        );

        Debug.Log($"Hit {target.name} for {damage} damage!");
    }

    private void DrawDebugShape(
        Vector2 origin,
        Vector2 forward,
        float debugDuration = 0.2f)
    {
        Vector2 right = new Vector2(forward.y, -forward.x);
        float rearHalfWidth =
            Mathf.Tan(tipAngle * 0.5f * Mathf.Deg2Rad) * length;

        Vector2 rearLeft = origin - right * rearHalfWidth;
        Vector2 rearRight = origin + right * rearHalfWidth;
        Vector2 tip = origin + forward * length;

        Debug.DrawLine(rearLeft, rearRight, Color.yellow, debugDuration);
        Debug.DrawLine(rearLeft, tip, Color.yellow, debugDuration);
        Debug.DrawLine(rearRight, tip, Color.yellow, debugDuration);
        Debug.DrawLine(origin, tip, Color.yellow, debugDuration);
    }
}
