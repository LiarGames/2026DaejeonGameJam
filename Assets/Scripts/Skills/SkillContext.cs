using UnityEngine;

public struct SkillContext
{
    public GameObject Caster;
    public float AttackPower;
    public Vector2 Direction;
    public LayerMask TargetLayer;
    public SkillModifiers Modifiers;
}
