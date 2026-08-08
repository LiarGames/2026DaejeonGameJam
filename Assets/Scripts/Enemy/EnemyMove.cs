using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyStats), typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D player;
    [SerializeField] private Rigidbody2D enemyself;

    [Header("Attack")]
    [SerializeField] private AttackSkill attackSkill;
    [SerializeField] private AttackSkill closeRangeAttackSkill;
    [Min(0f)]
    [SerializeField] private float closeRangeAttackDistance = 2f;
    [SerializeField] private LayerMask targetLayer;

    private EnemyStats stats;
    private float fireTimer;
    private bool isAttacking;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();

        if (enemyself == null)
            enemyself = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (player != null)
            return;

        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();

        if (playerStats != null)
            player = playerStats.GetComponent<Rigidbody2D>();

        if (player == null)
        {
            Debug.LogError($"Player target was not found for {name}.", this);
            enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (isAttacking)
        {
            StopMovement();
            return;
        }

        Vector2 playerPosition = player.position;
        Vector2 currentPosition = enemyself.position;
        Vector2 difference = playerPosition - currentPosition;

        if (difference.sqrMagnitude < stats.AttackRange * stats.AttackRange)
        {
            StopMovement();

            fireTimer -= Time.fixedDeltaTime;
            AttackSkill selectedAttack =
                SelectAttack(difference.sqrMagnitude);

            if (fireTimer <= 0f && selectedAttack != null)
            {
                StartCoroutine(PerformAttack(
                    selectedAttack,
                    difference.normalized
                ));
                fireTimer = stats.AttackCooldown;
            }
            return;
        }

        enemyself.linearVelocity = difference.normalized * stats.MoveSpeed;
    }

    private AttackSkill SelectAttack(float squaredDistance)
    {
        float closeRangeSquared =
            closeRangeAttackDistance * closeRangeAttackDistance;

        if (closeRangeAttackSkill != null &&
            squaredDistance <= closeRangeSquared)
        {
            return closeRangeAttackSkill;
        }

        return attackSkill;
    }

    private IEnumerator PerformAttack(
        AttackSkill selectedAttack,
        Vector2 direction)
    {
        isAttacking = true;
        StopMovement();

        try
        {
            yield return new WaitForSeconds(selectedAttack.ProcessDuration);

            SkillContext context = new SkillContext
            {
                Caster = gameObject,
                AttackPower = stats.Attack,
                Direction = direction,
                TargetLayer = targetLayer,
                Modifiers = SkillModifiers.Default
            };

            selectedAttack.Activate(context);

            yield return new WaitForSeconds(selectedAttack.RecoveryDuration);
        }
        finally
        {
            isAttacking = false;
        }
    }

    private void OnDisable()
    {
        isAttacking = false;
    }

    private void StopMovement()
    {
        enemyself.linearVelocity = Vector2.zero;
    }
}
