using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerStateController))]
public class PlayerSkillController : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private List<Skill> _equippedSkills = new List<Skill>();
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerStateController _stateController;
    [SerializeField] private LayerMask enemyLayer;
    
    [SerializeField] private float _turnInterval = 1f;
    private int _currentSkillIndex;
    private float _skillTimer;
    private SkillModifiers _pendingModifiers = SkillModifiers.Default;

    private float _attackDuration; // 이번 시전의 총 소요(선딜+후딜)
    private float _attackElapsed;  // 시전 경과

    private void Awake()
    {
        if (_stateController == null)
            _stateController = GetComponent<PlayerStateController>();
    }

    private void Update()
    {
        // 시전 중에는 스킬 타이머가 멈추므로, 회전 동기화를 위해 시전 경과를 따로 센다.
        if (_stateController.CurrentState == PlayerState.Attacking)
            _attackElapsed += Time.deltaTime;

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
        if (_equippedSkills.Count == 0)
            return null;

        if (_currentSkillIndex >= _equippedSkills.Count)
            _currentSkillIndex = 0;

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
            _attackDuration = 0f; // 시전 없음
            _attackElapsed = 0f;
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

        // 회전 동기화용: 이번 시전의 총 길이를 미리 기록한다.
        _attackDuration = effectiveProcessDuration
            + skill.RecoveryDuration / castSpeedMultiplier;
        _attackElapsed = 0f;

        _stateController.ChangeState(PlayerState.Attacking);
        _playerMovement.StopMovement();

        yield return new WaitForSeconds(effectiveProcessDuration);

        skill.Activate(CreateSkillContext(modifiers));
        _pendingModifiers = SkillModifiers.Default;

        float effectiveRecoveryDuration = 
            skill.RecoveryDuration / castSpeedMultiplier;

        yield return new WaitForSeconds(effectiveRecoveryDuration);

        while (_stateController.CurrentState == PlayerState.Dashing)
            yield return null;

        if (_stateController.CurrentState == PlayerState.Attacking)
            _stateController.ChangeState(PlayerState.Idle);
    }

    private SkillContext CreateSkillContext(SkillModifiers modifiers)
    {   
        Vector2 attackDirection = GetTargetDirection();

        return new SkillContext
        {
            Caster = gameObject,
            AttackPower = _playerStats.Attack,
            Direction = attackDirection,
            TargetLayer = enemyLayer,
            Modifiers = modifiers
        };
    }

    private Vector2 GetTargetDirection()
{
    EnemyStats[] enemies =
        FindObjectsByType<EnemyStats>(FindObjectsSortMode.None);

    Vector2 playerPosition = transform.position;

    Vector2 closestPosition = Vector2.zero;
    float closestDistanceSquared = Mathf.Infinity;
    bool foundEnemy = false;

    foreach (EnemyStats enemy in enemies)
    {
        if (enemy == null || !enemy.gameObject.activeInHierarchy)
            continue;

        Collider2D enemyCollider =
            enemy.GetComponent<Collider2D>();

        Vector2 enemyPosition =
            enemyCollider != null
                ? enemyCollider.bounds.center
                : enemy.transform.position;

        float distanceSquared =
            (enemyPosition - playerPosition).sqrMagnitude;

        if (distanceSquared >= closestDistanceSquared)
            continue;

        closestDistanceSquared = distanceSquared;
        closestPosition = enemyPosition;
        foundEnemy = true;
    }

    if (!foundEnemy)
        return _playerMovement.LastMoveDirection;

    Vector2 direction =
        closestPosition - playerPosition;

    if (direction.sqrMagnitude <= 0.001f)
        return _playerMovement.LastMoveDirection;

    return direction.normalized;
}
    
    private void AdvanceToNextSkill()
    {
        if (_equippedSkills.Count == 0) return;

        int checkedSlots = 0;

        do
        {
            _currentSkillIndex++;

            if (_currentSkillIndex >= _equippedSkills.Count)
                _currentSkillIndex = 0;

            checkedSlots++;
        }
        while (_equippedSkills[_currentSkillIndex] == null
            && checkedSlots < _equippedSkills.Count);
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

    // --- 레벨업 카드 삽입용 외부 API ---

    // 스킬 목록이 바뀌면 발생. 카루셀 UI가 구독해 다시 그린다.
    public event Action OnSkillsChanged;

    // 현재 발동 순서상 가리키는 스킬 인덱스 (하이라이트 등에 사용 가능).
    public int CurrentSkillIndex => _currentSkillIndex;

    // 한 사이클(시전 + 대기)의 진행도 0~1. 카루셀 회전 동기화에 사용한다.
    // 시전 중에도 진행되므로 회전이 멈추지 않는다.
    public float TurnProgress
    {
        get
        {
            float total = _attackDuration + _turnInterval;
            if (total <= 0f)
                return 0f;

            float elapsed = _stateController.CurrentState == PlayerState.Attacking
                ? _attackElapsed
                : _attackDuration + _skillTimer;

            return Mathf.Clamp01(elapsed / total);
        }
    }

    public int SkillCount => _equippedSkills.Count;

    public Skill GetSkill(int index)
    {
        if (index < 0 || index >= _equippedSkills.Count)
            return null;

        return _equippedSkills[index];
    }

    // 지정한 위치에 새 카드를 끼워넣는다. (교체가 아니라 삽입, 뒤 스킬들은 밀림)
    // insertIndex 유효 범위: 0 ~ Count (Count면 맨 뒤).
    public void InsertSkill(int insertIndex, Skill skill)
    {
        if (skill == null)
            return;

        insertIndex = Mathf.Clamp(insertIndex, 0, _equippedSkills.Count);
        _equippedSkills.Insert(insertIndex, skill);

        // 현재 재생 위치 앞에 끼워졌으면, 같은 스킬을 계속 가리키도록 인덱스 보정.
        if (insertIndex <= _currentSkillIndex)
            _currentSkillIndex++;

        OnSkillsChanged?.Invoke();
    }
}
