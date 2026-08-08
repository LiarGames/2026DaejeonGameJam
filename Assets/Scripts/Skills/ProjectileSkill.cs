using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Projectile Skill")]
public class ProjectileSkill : Skill
{   
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    
    public override void Activate(SkillContext context)
    {
        Vector2 playerPosition = context.Caster.transform.position;

        GameObject projectile = Instantiate(
            projectilePrefab,
            playerPosition,
            Quaternion.identity
        );

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        PlayerStats playerStats = context.Caster.GetComponent<PlayerStats>();
        Projectile projectileDamage = projectile.GetComponent<Projectile>();
        if (playerStats != null && projectileDamage != null)
        {
            float damage = playerStats.Attack * damageMultiplier;
            projectileDamage.Initialize(damage, ProjectileTarget.Enemy);
        }

        if (rb != null)
            rb.linearVelocity = context.Direction * projectileSpeed;
    }
}
