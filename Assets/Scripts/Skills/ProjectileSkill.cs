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

        rb.linearVelocity = context.Direction * projectileSpeed;
    }
}