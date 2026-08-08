using UnityEngine;


public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D player;
    [SerializeField] private Rigidbody2D enemyself;
    [SerializeField] private float attackRange = 5f;

    [Header("Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float fireCooldown = 1f;

    [Header("Melee")]
    [SerializeField] private GameObject meleeHitboxPrefab;
    [SerializeField] private float meleeDamage = 10f;
    [SerializeField] private float meleeOffset = 1f; // 적 앞쪽으로 히트박스를 얼마나 내보낼지

    private float fireTimer;



    private void Awake()
    {
        if (enemyself == null)
            enemyself = GetComponent<Rigidbody2D>();
    }

    // 스포너가 스폰 직후 호출: 플레이어 연결 + 난이도 배율 적용.
    public void Initialize(Rigidbody2D target, float speedMultiplier, float damageMultiplier)
    {
        player = target;
        moveSpeed *= speedMultiplier;
        meleeDamage *= damageMultiplier;
        projectileSpeed *= speedMultiplier;
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        Vector2 playerPosition = player.position;
        Vector2 currentPosition = enemyself.position;
        Vector2 difference = playerPosition - currentPosition;

        if (difference.sqrMagnitude < attackRange * attackRange)
        {
            enemyself.linearVelocity = Vector2.zero;

            fireTimer -= Time.fixedDeltaTime;
            if (fireTimer <= 0f)
            {
                if (attackRange > 2f)
                    FireProjectile(difference.normalized);
                else
                    MeleeAttack(difference.normalized);
                fireTimer = fireCooldown;
            }
            return;
        }

        enemyself.linearVelocity = difference.normalized * moveSpeed;
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
    }
    private void MeleeAttack(Vector2 direction)
    {
        if (meleeHitboxPrefab == null)
            return;

        // 공격 방향 앞쪽에 히트박스를 생성.
        Vector2 spawnPos = enemyself.position + direction * meleeOffset;

        // 히트박스가 플레이어 쪽을 향하도록 회전.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        GameObject hitbox = Instantiate(meleeHitboxPrefab, spawnPos, rotation);

        MeleeHitbox melee = hitbox.GetComponent<MeleeHitbox>();
        if (melee != null)
            melee.Initialize(meleeDamage);
    }
}
