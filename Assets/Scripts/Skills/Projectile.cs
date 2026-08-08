using UnityEngine;

public enum ProjectileTarget
{
    Player,
    Enemy
}

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;

    private float damage;
    private ProjectileTarget targetType;
    private bool hasHit;

    public void Initialize(float damage, ProjectileTarget targetType)
    {
        this.damage = damage;
        this.targetType = targetType;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
            return;

        if (targetType == ProjectileTarget.Player)
        {
            PlayerStats playerStats = other.GetComponentInParent<PlayerStats>();
            if (playerStats == null)
                return;

            playerStats.TakeDamage(damage);
        }
        else
        {
            EnemyStats enemyStats = other.GetComponentInParent<EnemyStats>();
            if (enemyStats == null)
                return;

            enemyStats.TakeDamage(damage);
        }

        hasHit = true;
        Destroy(gameObject);
    }
}
