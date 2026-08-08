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
    private SkillModifiers _pendingModifiers = SkillModifiers.Default;

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

        Skill skill = GetCurrentSkill();

        if (skill is AttackSkill && !_stateController.CanAttack())
            return;

        _skillTimer = 0f;
        ProcessCurrentSkill(skill);
    }

    private Skill GetCurrentSkill()
    {
        Skill skill = _equippedSkills[_currentSkillIndex];

        if (skill == null)
        {
            AdvanceToNextSkill();
            skill = _equippedSkills[_currentSkillIndex];
        }

        return skill;
    }

    private void ProcessCurrentSkill(Skill skill)
    {
        if (skill is SupportSkill supportSkill)
        {
            supportSkill.ApplyModifier(ref _pendingModifiers);
            AdvanceToNextSkill();
            return;
        }

        if (skill is AttackSkill attackSkill)
        {
            StartCoroutine(PerformSkill(attackSkill));
            AdvanceToNextSkill();
        }
    }

    private IEnumerator PerformSkill(AttackSkill skill)
    {
        SkillModifiers modifiers = _pendingModifiers;
        float castSpeedMultiplier =
            Mathf.Max(0.01f, modifiers.CastSpeedMultiplier);
        float effectiveProcessDuration =
            skill.ProcessDuration / castSpeedMultiplier;

        _stateController.ChangeState(PlayerState.Attacking);
        _playerMovement.StopMovement();

        yield return new WaitForSeconds(effectiveProcessDuration);

        skill.Activate(CreateSkillContext(modifiers));
        _pendingModifiers = SkillModifiers.Default;

        float effectiveRecoveryDuration = 
            skill.RecoveryDuration / castSpeedMultiplier;

        yield return new WaitForSeconds(effectiveRecoveryDuration);

        if (_stateController.CurrentState == PlayerState.Attacking)
            _stateController.ChangeState(PlayerState.Idle);
    }

    private SkillContext CreateSkillContext(SkillModifiers modifiers)
    {
        return new SkillContext
        {
            Caster = gameObject,
            AttackPower = _playerStats.Attack,
            Direction = _playerMovement.LastMoveDirection,
            TargetLayer = enemyLayer,
            Modifiers = modifiers
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
