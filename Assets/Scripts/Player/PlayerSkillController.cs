using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [SerializeField] private Skill[] equippedSkills = new Skill[4];

    public void UseSkill(int slot)
    {
        if (slot < 0 || slot >= equippedSkills.Length)
            return;

        Skill skill = equippedSkills[slot];

        if (skill == null)
            return;

        skill.Activate(gameObject);
    }
    
}
