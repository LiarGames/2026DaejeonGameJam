using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [SerializeField] private Skill[] equippedSkills = new Skill[4];

    private float [] cooldownTimers = new float[4];

    private void Update()
    {
        CountTimers();
    }

    public void UseSkill(int slot, Vector2 targetPosition)
    {
        if (slot < 0 || slot >= equippedSkills.Length)
            return;

        Skill skill = equippedSkills[slot];

        if (skill == null)
            return;

        if (cooldownTimers[slot] > 0)
            return;


        skill.Activate(gameObject, targetPosition);
        cooldownTimers[slot] = skill.Cooldown;
    }

    private void CountTimers()
    {
        // cooldown timer
        for (int i = 0; i < cooldownTimers.Length; i++)
        {
            if (cooldownTimers[i] > 0)
                cooldownTimers[i] -= Time.deltaTime;
        }
    }
    
}
