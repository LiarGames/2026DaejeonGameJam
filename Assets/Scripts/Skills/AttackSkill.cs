using UnityEngine;

public abstract class AttackSkill : Skill
{
    [SerializeField] protected float damageMultiplier = 1f;
    [SerializeField] private float processDuration;
    [SerializeField] private float recoveryDuration;

    [Header("Cast Origin")]
    [Tooltip("X moves forward/backward and Y moves sideways relative to the cast direction.")]
    [SerializeField] private Vector2 castOffset;

    public float ProcessDuration => processDuration;
    public float RecoveryDuration => recoveryDuration;

    public abstract void Activate(SkillContext context);

    protected Vector2 GetCastPosition(SkillContext context)
    {
        Vector2 forward = context.Direction.sqrMagnitude > 0.001f
            ? context.Direction.normalized
            : Vector2.right;
        Vector2 right = new Vector2(forward.y, -forward.x);

        return GetSkillOriginPosition(context) +
            forward * castOffset.x +
            right * castOffset.y;
    }

}
