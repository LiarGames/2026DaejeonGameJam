using UnityEngine;

[CreateAssetMenu(menuName = "Skills/AoE Skill")]
public class AoESkill : AttackSkill
{
    [SerializeField] private float castDistance = 3f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float duration = 3f;
    [SerializeField] private float tickInterval = 0.5f;
    [SerializeField] private AoEZone zonePrefab;

    public float CastDistance => castDistance;
    public float Radius => radius;

    public override void Activate(SkillContext context)
    {
        if (zonePrefab == null)
        {
            Debug.LogWarning("Persistent AoE Skill has no zone prefab assigned.");
            return;
        }

        Vector2 spawnPosition =
            (Vector2)context.Caster.transform.position +
            context.Direction.normalized * castDistance;

        float damagePerTick =
            context.Stats.Attack * damageMultiplier;

        AoEZone zone = Instantiate(
            zonePrefab,
            spawnPosition,
            Quaternion.identity
        );

        zone.Initialize(
            radius,
            duration,
            tickInterval,
            damagePerTick,
            context.EnemyLayer,
            context.Modifiers
        );

        DrawDebugShape(
            context.Caster.transform.position,
            spawnPosition
        );
    }

    private void DrawDebugShape(
        Vector2 origin,
        Vector2 center,
        float debugDuration = 0.2f)
    {
        const int segments = 32;

        Debug.DrawLine(origin, center, Color.cyan, debugDuration);

        Vector2 previousPoint = center + Vector2.right * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector2 point = center + new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * radius;

            Debug.DrawLine(
                previousPoint,
                point,
                Color.cyan,
                debugDuration
            );

            previousPoint = point;
        }
    }
}
