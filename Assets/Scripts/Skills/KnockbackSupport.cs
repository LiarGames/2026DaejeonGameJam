using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Support/Knockback Support")]
public class KnockbackSupport : SupportSkill
{
    [SerializeField] private float knockbackDistance = 0.5f;

    public override void ApplyModifier(ref SkillModifiers modifiers)
    {
        modifiers.KnockbackDistance += knockbackDistance;
    }
}
