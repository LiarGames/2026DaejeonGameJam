using System.Collections;
using Spine.Unity;
using UnityEngine;

public enum EnemyMovementType
{
    Melee,
    Ranged
}

[RequireComponent(typeof(EnemyStats), typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D player;
    [SerializeField] private Rigidbody2D enemyself;

    [Header("Movement")]
    [SerializeField] private EnemyMovementType movementType;

    [Header("Facing")]
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool defaultFacesLeft = true;

    [Header("Ranged Movement")]
    [SerializeField] private float preferredRange = 5f;
    [SerializeField] private float rangeTolerance = 0.25f;

    [Header("Attack")]
    [SerializeField] private AttackSkill attackSkill;
    [SerializeField] private Transform skillOrigin;
    [SerializeField] private LayerMask targetLayer;

    private EnemyStats stats;
    private Collider2D movementBounds;
    private float fireTimer;
    private bool isAttacking;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();

        if (enemyself == null)
            enemyself = GetComponent<Rigidbody2D>();

        if (skeletonAnimation == null)
            skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        PlayerMovement playerMovement = null;

        if (player != null)
            playerMovement = player.GetComponent<PlayerMovement>();
        else
        {
            PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();

            if (playerStats != null)
            {
                player = playerStats.GetComponent<Rigidbody2D>();
                playerMovement = playerStats.GetComponent<PlayerMovement>();
            }
        }

        if (playerMovement == null)
            playerMovement = FindFirstObjectByType<PlayerMovement>();

        if (playerMovement != null)
            movementBounds = playerMovement.MovementBounds;

        if (player == null)
        {
            Debug.LogError(
                $"Player target was not found for {name}.",
                this
            );

            enabled = false;
        }
    }

    private void FixedUpdate()
    {
        Vector2 difference =
            player.position - enemyself.position;

        UpdateFacing(difference);

        if (isAttacking)
        {
            StopMovement();
            return;
        }

        float squaredDistance =
            difference.sqrMagnitude;

        fireTimer -= Time.fixedDeltaTime;

        HandleMovement(
            difference,
            squaredDistance
        );

        HandleAttack(
            difference,
            squaredDistance
        );
    }

    private void HandleMovement(
        Vector2 difference,
        float squaredDistance)
    {
        switch (movementType)
        {
            case EnemyMovementType.Melee:
                HandleMeleeMovement(
                    difference,
                    squaredDistance
                );
                break;

            case EnemyMovementType.Ranged:
                HandleRangedMovement(
                    difference,
                    squaredDistance
                );
                break;
        }
    }

    private void HandleMeleeMovement(
        Vector2 difference,
        float squaredDistance)
    {
        float attackRangeSquared =
            stats.AttackRange * stats.AttackRange;

        if (squaredDistance <= attackRangeSquared)
        {
            StopMovement();
            return;
        }

        enemyself.linearVelocity =
            difference.normalized * stats.MoveSpeed;
    }

    private void HandleRangedMovement(
        Vector2 difference,
        float squaredDistance)
    {
        if (!IsInsideMovementBounds())
        {
            enemyself.linearVelocity =
                difference.normalized * stats.MoveSpeed;
            return;
        }

        float maxRange =
            preferredRange + rangeTolerance;

        float maxRangeSquared =
            maxRange * maxRange;

        if (squaredDistance > maxRangeSquared)
        {
            enemyself.linearVelocity =
                difference.normalized * stats.MoveSpeed;
        }
        else
        {
            StopMovement();
        }
    }

    private void HandleAttack(
        Vector2 difference,
        float squaredDistance)
    {
        if (movementType == EnemyMovementType.Ranged &&
            !IsInsideMovementBounds())
            return;

        float attackRangeSquared =
            stats.AttackRange * stats.AttackRange;

        if (squaredDistance > attackRangeSquared)
            return;

        if (fireTimer > 0f)
            return;

        if (attackSkill == null)
            return;

        StartCoroutine(
            PerformAttack(
                attackSkill,
                difference.normalized
            )
        );

        fireTimer = stats.AttackCooldown;
    }

    private IEnumerator PerformAttack(
        AttackSkill selectedAttack,
        Vector2 direction)
    {
        isAttacking = true;
        StopMovement();

        try
        {
            yield return new WaitForSeconds(
                selectedAttack.ProcessDuration
            );

            SkillContext context =
                new SkillContext
                {
                    Caster = gameObject,
                    CastOrigin = skillOrigin,
                    FacingDirection = direction,
                    AttackPower = stats.Attack,
                    Direction = direction,
                    TargetLayer = targetLayer,
                    Modifiers = SkillModifiers.Default
                };

            selectedAttack.Activate(context);

            yield return new WaitForSeconds(
                selectedAttack.RecoveryDuration
            );
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
        enemyself.linearVelocity =
            Vector2.zero;
    }

    private void UpdateFacing(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) <= 0.01f)
            return;

        bool facesLeft = direction.x < 0f;
        bool shouldFlip = facesLeft != defaultFacesLeft;

        if (skeletonAnimation != null &&
            skeletonAnimation.Skeleton != null)
        {
            float magnitude = Mathf.Abs(
                skeletonAnimation.Skeleton.ScaleX
            );

            skeletonAnimation.Skeleton.ScaleX =
                shouldFlip ? -magnitude : magnitude;
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.flipX = shouldFlip;
        }
    }

    private bool IsInsideMovementBounds()
    {
        if (movementBounds == null)
            return true;

        return movementBounds.bounds.Contains(enemyself.position);
    }
}
