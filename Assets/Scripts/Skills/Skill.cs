using UnityEngine;

public abstract class Skill : ScriptableObject
{
    [SerializeField] protected float damageMultiplier = 1f;
    [SerializeField] private float processDuration;
    [SerializeField] private float recoveryDuration;

    public float ProcessDuration => processDuration;
    public float RecoveryDuration => recoveryDuration;

    public abstract void Activate(SkillContext context);
}
