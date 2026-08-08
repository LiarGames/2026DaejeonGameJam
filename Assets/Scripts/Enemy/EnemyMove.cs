using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D player;
    [SerializeField] private Rigidbody2D enemyself;

    [Header("Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;

    [Header("Melee")]
    [SerializeField] private GameObject meleeHitboxPrefab;
    [SerializeField] private float meleeOffset = 1f;

    private EnemyStats stats;
    private float fireTimer;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
    }

    private void FixedUpdate()
    {
        Vector2 playerPosition = player.position;
        Vector2 currentPosition = enemyself.position;
        Vector2 difference = playerPosition - currentPosition;

        if (difference.sqrMagnitude < stats.AttackRange * stats.AttackRange)
        {
            enemyself.linearVelocity = Vector2.zero;

            fireTimer -= Time.fixedDeltaTime;
            if (fireTimer <= 0f)
            {
                if (stats.AttackRange > 2f)
                    FireProjectile(difference.normalized);
                else
                    MeleeAttack(difference.normalized);

                fireTimer = stats.AttackCooldown;
            }
            return;
        }

        enemyself.linearVelocity = difference.normalized * stats.MoveSpeed;
    }

    private void FireProjectile(Vector2 direction)
    {
        if (projectilePrefab == null)
            return;

        GameObject projectile = Instantiate(
            projectilePrefab,
            enemyself.position,
            Quaternion.identity
        );

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction * projectileSpeed;

        Projectile projectileDamage = projectile.GetComponent<Projectile>();
        if (projectileDamage != null)
            projectileDamage.Initialize(stats.Attack, ProjectileTarget.Player);
    }

    private void MeleeAttack(Vector2 direction)
    {
        if (meleeHitboxPrefab == null)
            return;

        Vector2 spawnPos = enemyself.position + direction * meleeOffset;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        GameObject hitbox = Instantiate(meleeHitboxPrefab, spawnPos, rotation);

        MeleeHitbox melee = hitbox.GetComponent<MeleeHitbox>();
        if (melee != null)
            melee.Initialize(stats.Attack);
    }
}
