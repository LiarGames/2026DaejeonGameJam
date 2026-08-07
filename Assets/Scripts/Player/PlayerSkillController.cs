using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerStateController))]
public class PlayerSkillController : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private Skill[] _equippedSkills = new Skill[8];
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerStateController _stateController;
    [SerializeField] private LayerMask enemyLayer;
    
    [SerializeField] private float _turnInterval = 1f;
    private int _currentSkillIndex;
    private float _skillTimer;

    private void Awake()
    {
        if (_stateController == null)
            _stateController = GetComponent<PlayerStateController>();
    }

    private void Update()
    {
        CountSkillTimer();
    }

    private void CountSkillTimer()
    {
        if (!HasAnySkill() || !_stateController.CanAttack())
            return;

        _skillTimer += Time.deltaTime;

        if (_skillTimer < _turnInterval)
            return;

        _skillTimer = 0f;

        StartCurrentSkill();
        AdvanceToNextSkill();
    }

    private void StartCurrentSkill()
    {
        Skill skill = _equippedSkills[_currentSkillIndex];

        if (skill == null)
            return;

        StartCoroutine(PerformSkill(skill));
    }

    private IEnumerator PerformSkill(Skill skill)
    {
        _stateController.ChangeState(PlayerState.Attacking);
        _playerMovement.StopMovement();

        yield return new WaitForSeconds(skill.ProcessDuration);

        skill.Activate(CreateSkillContext());

        yield return new WaitForSeconds(skill.RecoveryDuration);

        if (_stateController.CurrentState == PlayerState.Attacking)
            _stateController.ChangeState(PlayerState.Idle);
    }

    private SkillContext CreateSkillContext()
    {
        return new SkillContext
        {
            Caster = gameObject,
            Stats = _playerStats,
            Direction = _playerMovement.LastMoveDirection,
            EnemyLayer = enemyLayer
        };
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
