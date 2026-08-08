using UnityEngine;

public struct SkillContext
{
    public GameObject Caster;
    public Transform CastOrigin;
    public Vector2 FacingDirection;
    public float AttackPower;
    public Vector2 Direction;
    public LayerMask TargetLayer;
    public SkillModifiers Modifiers;
}
