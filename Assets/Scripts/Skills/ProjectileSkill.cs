using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Projectile Skill")]
public class ProjectileSkill : AttackSkill
{   
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private int baseProjectileCount = 1;
    [SerializeField] private float projectileSpreadAngle = 15f;
    
    public override void Activate(SkillContext context)
    {
        int projectileCount = Mathf.Max(
            1,
            Mathf.RoundToInt(
                baseProjectileCount *
                context.Modifiers.ProjectileCountMultiplier
            )
        );

        for (int i = 0; i < projectileCount; i++)
            SpawnProjectile(context, i, projectileCount);
    }

    private void SpawnProjectile(
        SkillContext context,
        int projectileIndex,
        int projectileCount)
    {
        Vector2 spawnPosition = GetCastPosition(context);
        float centerOffset = (projectileCount - 1) * 0.5f;
        float angleOffset =
            (projectileIndex - centerOffset) * projectileSpreadAngle;
        Vector2 direction =
            Quaternion.Euler(0f, 0f, angleOffset) *
            context.Direction.normalized;

        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        Projectile projectileComponent =
            projectile.GetComponent<Projectile>();
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (projectileComponent == null || rb == null)
        {
            Debug.LogError(
                "Projectile prefab requires Projectile and Rigidbody2D components."
            );
            Destroy(projectile);
            return;
        }

        float damage = context.AttackPower * damageMultiplier;

        projectileComponent.Initialize(
            damage,
            context.TargetLayer,
            context.Modifiers
        );

        SpawnVFX(
            spawnPosition,
            direction,
            projectile.transform
        );

        rb.linearVelocity = direction * projectileSpeed;
    }
}
