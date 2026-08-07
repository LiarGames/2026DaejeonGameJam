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

        float damage = context.Stats.Attack * damageMultiplier;

        projectileComponent.Initialize(damage, context.EnemyLayer);
        rb.linearVelocity = context.Direction.normalized * projectileSpeed;
    }
}
