using UnityEngine;

public abstract class Skill : ScriptableObject
{
    [SerializeField] protected float damageMultiplier = 1f;

    public abstract void Activate(SkillContext context);
}