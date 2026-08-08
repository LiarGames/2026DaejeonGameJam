using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(EnemyStats))]
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => _stats != null ? _stats.MaxHealth : 0f;

    private EnemyStats _stats;
    private bool _isDead;

    private void Awake()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();

        _stats = GetComponent<EnemyStats>();
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (_isDead)
            return;

        float appliedDamage = Mathf.Max(damage - _stats.Defense, 0f);
        CurrentHealth = Mathf.Max(CurrentHealth - appliedDamage, 0f);

        Debug.Log($"{name} took {appliedDamage} damage.", this);

        if (CurrentHealth <= 0f)
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

    // TODO: update knockback logic later for smoother knockback
    public void ApplyKnockback(Vector2 hitOrigin, float distance)
    {
        Vector2 direction = (_rb.position - hitOrigin).normalized;
        _rb.position += direction * distance;
    }
}
