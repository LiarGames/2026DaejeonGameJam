using UnityEngine;

[CreateAssetMenu(menuName = "Skills/AoE Skill")]
public class AoESkill : AttackSkill
{
    [SerializeField] private float castDistance = 3f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float duration = 3f;
    [SerializeField] private float tickInterval = 0.5f;
    [SerializeField] private GameObject zonePrefab;

    public float CastDistance => castDistance;
    public float Radius => radius;

    public override void Activate(SkillContext context)
    {
        if (zonePrefab == null)
        {
            Debug.LogWarning("Persistent AoE Skill has no zone prefab assigned.");
            return;
        }

        Vector2 castOrigin = GetCastPosition(context);
        Vector2 spawnPosition =
            castOrigin +
            context.Direction.normalized * castDistance;

        float damagePerTick =
            context.AttackPower * damageMultiplier;

        GameObject zoneObject = Instantiate(
            zonePrefab,
            spawnPosition,
            Quaternion.identity
        );

        float effectiveRadius = radius * context.Modifiers.RangeMultiplier;

        AoEZone zone = zoneObject.GetComponent<AoEZone>();

        if (zone == null)
        {
            Debug.LogError(
                $"AoE prefab '{zonePrefab.name}' requires an AoEZone component " +
                "on its root GameObject.",
                zoneObject
            );
            Destroy(zoneObject);
            return;
        }

        zone.Initialize(
            effectiveRadius,
            duration,
            tickInterval,
            damagePerTick,
            context.TargetLayer,
            context.Modifiers
        );

        SpawnVFX(
            spawnPosition,
            context.Direction,
            zone.transform,
            Vector3.one * (effectiveRadius * 2f)
        );

        DrawDebugShape(
            castOrigin,
            spawnPosition,
            context.Modifiers
        );
    }

    private void DrawDebugShape(
        Vector2 origin,
        Vector2 center,
        SkillModifiers modifiers,
        float debugDuration = 0.2f)
    {
        const int segments = 32;

        Debug.DrawLine(origin, center, Color.cyan, debugDuration);

        Vector2 previousPoint = center + Vector2.right * radius * modifiers.RangeMultiplier;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector2 point = center + new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * radius * modifiers.RangeMultiplier;

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
