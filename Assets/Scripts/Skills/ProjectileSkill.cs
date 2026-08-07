using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Projectile Skill")]
public class ProjectileSkill : Skill
{   
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    
    public override void Activate(GameObject player, Vector2 direction)
    {
        Vector2 playerPosition = player.transform.position;


        GameObject projectile = Instantiate(
            projectilePrefab,
            playerPosition,
            Quaternion.identity
        );

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        rb.linearVelocity = direction * projectileSpeed;
    }
}