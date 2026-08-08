using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Dash Skill")]
public class DashSkill : AttackSkill
{
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.2f;

    public override void Activate(SkillContext context)
    {
        if (context.Direction.sqrMagnitude <= 0.001f)
        {
            Debug.LogWarning("Dash requires a non-zero direction.");
            return;
        }

        Rigidbody2D rb = context.Caster.GetComponent<Rigidbody2D>();
        DashRunner dashRunner = context.Caster.GetComponent<DashRunner>();

        if (rb == null || dashRunner == null)
        {
            Debug.LogError(
                $"{context.Caster.name} requires Rigidbody2D and " +
                "DashRunner components to use Dash Skill.",
                context.Caster
            );
            return;
        }

        dashRunner.StartDash(
            rb,
            context.Direction.normalized,
            dashSpeed,
            dashDuration
        );

        SpawnVFX(
            context.Caster.transform.position,
            context.Direction,
            context.Caster.transform
        );
    }
}
