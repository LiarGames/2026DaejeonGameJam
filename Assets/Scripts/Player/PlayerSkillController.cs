using Unity.VisualScripting;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private Skill[] _equippedSkills = new Skill[8];
    [SerializeField] private PlayerMovement _playerMovement;
    
    [SerializeField] private float _turnInterval = 1f;
    private int _currentSkillIndex;
    private float _skillTimer;

    private void Update()
    {
        CountSkillTimer();
    }

    private void CountSkillTimer()
    {
        if (!HasAnySkill())
            return;

        _skillTimer += Time.deltaTime;

        if (_skillTimer < _turnInterval)
            return;

        _skillTimer = 0f;

        ActivateCurrentSkill();
        AdvanceToNextSkill();
    }

    private void ActivateCurrentSkill()
    {
        Skill skill = _equippedSkills[_currentSkillIndex];

        if (skill == null)
            return;

        skill.Activate(gameObject, _playerMovement.LastMoveDirection);
    }
    
    private void AdvanceToNextSkill()
    {
        if (_equippedSkills.Length == 0) return;

        int checkedSlots = 0;

        do
        {
            _currentSkillIndex++;

            if (_currentSkillIndex >= _equippedSkills.Length)
                _currentSkillIndex = 0;

            checkedSlots++;
        }
        while (_equippedSkills[_currentSkillIndex] == null
            && checkedSlots < _equippedSkills.Length);
    }

    private bool HasAnySkill()
    {
        foreach (Skill skill in _equippedSkills)
        {
            if (skill != null)
                return true;
        }

        return false;
    }
}
