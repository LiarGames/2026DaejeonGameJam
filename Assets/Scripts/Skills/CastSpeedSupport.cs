using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Support/Cast Speed Support")]
public class CastSpeedSupport : SupportSkill
{
    [SerializeField] private float castSpeedMultiplier = 2f;

    public override void ApplyModifier(ref SkillModifiers modifiers)
    {
        modifiers.CastSpeedMultiplier *= castSpeedMultiplier;
    }
}
