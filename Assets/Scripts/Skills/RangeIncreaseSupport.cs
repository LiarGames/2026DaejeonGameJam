using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Support/Range Increase")]
public class RangeIncreaseSupport : SupportSkill
{
    [SerializeField] private float rangeMultiplier = 2f;

    public override void ApplyModifier(ref SkillModifiers modifiers)
    {
        modifiers.RangeMultiplier *= rangeMultiplier;
    }
}