using Unity.VisualScripting;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Skill[] equippedSkills = new Skill[8];
    
    [SerializeField] private float turnInterval = 1f;
    private int _currentSkillIndex;
    private float _skillTimer;

    private Vector2 _lastMoveDirection = Vector2.right;

    private void Update()
    {
        CountSkillTimer();
    }

    private void CountSkillTimer()
    {
        if (!HasAnySkill())
            return;

        _skillTimer += Time.deltaTime;

        if (_skillTimer < turnInterval)
            return;

        _skillTimer = 0f;

        ActivateCurrentSkill();
        AdvanceToNextSkill();
    }

    private void ActivateCurrentSkill()
    {
        Skill skill = equippedSkills[_currentSkillIndex];

        if (skill == null)
            return;

        skill.Activate(gameObject, _lastMoveDirection);
    }
    
    private void AdvanceToNextSkill()
    {
        if (equippedSkills.Length == 0) return;

        int checkedSlots = 0;

        do
        {
            _currentSkillIndex++;

            if (_currentSkillIndex >= equippedSkills.Length)
                _currentSkillIndex = 0;

            checkedSlots++;
        }
        while (equippedSkills[_currentSkillIndex] == null
            && checkedSlots < equippedSkills.Length);
    }

    private bool HasAnySkill()
    {
        foreach (Skill skill in equippedSkills)
        {
            if (skill != null)
                return true;
        }

        return false;
    }

    public void SetLastMoveDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
            _lastMoveDirection = direction.normalized;
    }
}
