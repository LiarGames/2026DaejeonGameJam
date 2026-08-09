using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(EnemyStats))]
public class EnemyHealth : MonoBehaviour, IDamageable, IKnockbackable
{
    [SerializeField] private Rigidbody2D _rb;

    [Header("Hit Reaction")]
    [Min(0f)]
    [SerializeField] private float hitReactionDuration = 0.1f;

    [Header("Death Animation")]
    [Min(0f)]
    [SerializeField] private float deathAnimationDuration = 0.3f;
    [SerializeField] private float deathRotationDegrees = 360f;

    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => _stats != null ? _stats.MaxHealth : 0f;

    private EnemyStats _stats;
    private bool _isDead;
    private bool _isDying;
    private Coroutine _flashCoroutine;
    private Coroutine _knockbackCoroutine;
    private Material _flashMaterial;

    private static readonly int FlashAmountId =
        Shader.PropertyToID("_FlashAmount");

    private void Awake()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();

        _stats = GetComponent<EnemyStats>();

        if (enemyMovement == null)
            enemyMovement = GetComponent<EnemyMovement>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            Shader flashShader = Shader.Find("Game/Sprite White Flash");

            if (flashShader != null)
            {
                _flashMaterial = new Material(spriteRenderer.material)
                {
                    shader = flashShader
                };
                spriteRenderer.material = _flashMaterial;
                _flashMaterial.SetFloat(FlashAmountId, 0f);
            }
        }
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (_isDead || _isDying)
            return;

        float appliedDamage = Mathf.Max(damage - _stats.Defense, 0f);

        if (appliedDamage <= 0f)
            return;

        CurrentHealth = Mathf.Max(CurrentHealth - appliedDamage, 0f);

        if (enemyMovement != null)
            enemyMovement.Stun(hitReactionDuration);

        PlayHitFlash();

        Debug.Log($"{name} took {appliedDamage} damage.", this);

        if (CurrentHealth <= 0f)
        {
            _isDying = true;

            if (enemyMovement != null)
            {
                enemyMovement.Stun(
                    hitReactionDuration + deathAnimationDuration
                );
            }

            StartCoroutine(DeathSequence());
        }
    }

    private void PlayHitFlash()
    {
        if (_flashCoroutine != null)
            StopCoroutine(_flashCoroutine);

        if (_flashMaterial != null)
            _flashCoroutine = StartCoroutine(FlashSpriteRoutine());
    }

    private IEnumerator FlashSpriteRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < hitReactionDuration)
        {
            float progress = hitReactionDuration > 0f
                ? elapsedTime / hitReactionDuration
                : 1f;
            float whiteAmount = Mathf.Sin(progress * Mathf.PI);

            _flashMaterial.SetFloat(FlashAmountId, whiteAmount);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _flashMaterial.SetFloat(FlashAmountId, 0f);
        _flashCoroutine = null;
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(hitReactionDuration);

        Transform visual = spriteRenderer != null
            ? spriteRenderer.transform
            : transform;
        Vector3 startScale = visual.localScale;
        Quaternion startRotation = visual.localRotation;
        float elapsedTime = 0f;

        while (elapsedTime < deathAnimationDuration)
        {
            float progress = deathAnimationDuration > 0f
                ? elapsedTime / deathAnimationDuration
                : 1f;
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            visual.localScale = Vector3.Lerp(
                startScale,
                Vector3.zero,
                easedProgress
            );
            visual.localRotation = startRotation * Quaternion.Euler(
                0f,
                0f,
                deathRotationDegrees * easedProgress
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        visual.localScale = Vector3.zero;
        Die();
    }

    private void Die()
    {
        if (_isDead)
            return;

        _isDead = true;
        Debug.Log($"{name} died", this);

        if (_stats.ExperienceGemPrefab != null)
        {
            ExperienceGem gem = Instantiate(
                _stats.ExperienceGemPrefab,
                transform.position,
                Quaternion.identity
            );
            gem.Initialize(_stats.ExperienceReward);
        }
        else
        {
            Debug.LogWarning($"ExperienceGem prefab is not assigned to {name}.", this);
        }

        Destroy(gameObject);
    }

    public void ApplyKnockback(Vector2 hitOrigin, float distance)
    {
        if (_isDead || distance <= 0f)
            return;

        Vector2 direction = (_rb.position - hitOrigin).normalized;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        if (_knockbackCoroutine != null)
            StopCoroutine(_knockbackCoroutine);

        _knockbackCoroutine = StartCoroutine(
            KnockbackRoutine(direction, distance)
        );
    }

    private IEnumerator KnockbackRoutine(
        Vector2 direction,
        float distance)
    {
        Vector2 startPosition = _rb.position;
        Vector2 targetPosition = startPosition + direction * distance;
        float duration = Mathf.Max(0.01f, hitReactionDuration);
        float elapsedTime = 0f;
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        while (elapsedTime < duration)
        {
            float progress = Mathf.Clamp01(elapsedTime / duration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            _rb.position = Vector2.Lerp(
                startPosition,
                targetPosition,
                easedProgress
            );

            yield return waitForFixedUpdate;
            elapsedTime += Time.fixedDeltaTime;
        }

        _rb.position = targetPosition;
        _knockbackCoroutine = null;
    }

    private void OnDestroy()
    {
        if (_flashMaterial != null)
            Destroy(_flashMaterial);
    }
}
