using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Melee Skill")]
public class MeleeSkill : Skill
{
    [Header("Attack Shape")]
    [SerializeField] private float radius = 2f;

    [Range(0f, 360f)]
    [SerializeField] private float angle = 90f;

    public override void Activate(SkillContext context)
    {   
        Debug.Log("Melee Attack");
        Collider2D[] targets = Physics2D.OverlapCircleAll(
            context.Caster.transform.position,
            radius,
            context.EnemyLayer
        );

        foreach (Collider2D target in targets)
        {
            Vector2 directionToTarget =
                ((Vector2)target.transform.position -
                 (Vector2)context.Caster.transform.position).normalized;

            float targetAngle =
                Vector2.Angle(context.Direction, directionToTarget);

            if (targetAngle <= angle / 2f)
            {
                HitTarget(context.Caster, target);
            }
        }
    }

    private void HitTarget(GameObject player, Collider2D target)
    {
        PlayerStats playerStats =
            player.GetComponent<PlayerStats>();

        float damage =
            playerStats.Attack * damageMultiplier;

        Debug.Log(
            $"Hit {target.name} for {damage} damage!"
        );

        // Eventually:
        // target.GetComponent<EnemyHealth>()?.TakeDamage(damage);
    }
}