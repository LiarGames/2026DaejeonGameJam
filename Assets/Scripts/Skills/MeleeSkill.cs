using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Melee Skill")]
public class MeleeSkill : Skill
{
    [Header("Attack Shape")]
    [SerializeField] private float radius = 2f;

    [Range(0f, 360f)]
    [SerializeField] private float angle = 90f;

    [Header("Visual")]
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private float vfxLifetime = 0.15f;

    public override void Activate(SkillContext context)
    {   
        PerformAttack(context);
        SpawnVFX(context);

        Vector2 origin = context.Caster.transform.position;

        DrawDebugSector(
            origin,
            context.Direction,
            radius,
            angle
        );
    }

    private void SpawnVFX(SkillContext context)
    {
        if (vfxPrefab == null)
            return;
        
        Vector2 position = context.Caster.transform.position;

        float rotation =
            Mathf.Atan2(
                context.Direction.y,
                context.Direction.x
            ) * Mathf.Rad2Deg;

        GameObject vfx = Instantiate(
            vfxPrefab,
            position,
            Quaternion.Euler(0f, 0f, rotation)
        );

        vfx.transform.localScale = Vector3.one * radius;

        Destroy(vfx, vfxLifetime);
    }

    private void PerformAttack(SkillContext context)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(
            context.Caster.transform.position,
            radius,
            context.EnemyLayer
        );

        HashSet<EnemyStats> damagedTargets = new HashSet<EnemyStats>();

        foreach (Collider2D target in targets)
        {
            EnemyStats enemyStats = target.GetComponentInParent<EnemyStats>();
            if (enemyStats == null || !damagedTargets.Add(enemyStats))
                continue;

            Vector2 directionToTarget =
                ((Vector2)enemyStats.transform.position -
                 (Vector2)context.Caster.transform.position).normalized;

            float targetAngle =
                Vector2.Angle(context.Direction, directionToTarget);

            if (targetAngle <= angle / 2f)
            {
                HitTarget(context.Caster, enemyStats);
            }
        }
    }

    private void HitTarget(GameObject player, EnemyStats target)
    {
        PlayerStats playerStats =
            player.GetComponent<PlayerStats>();

        float damage =
            playerStats.Attack * damageMultiplier;

        target.TakeDamage(damage);

        //Debug.Log($"Hit {target.name} for {damage} damage!");
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
