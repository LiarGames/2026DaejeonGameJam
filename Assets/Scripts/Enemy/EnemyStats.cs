using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private EnemyStatsData data;
    [SerializeField] private ExperienceGem experienceGemPrefab;

    public float Attack { get; private set; }
    public float Defense { get; private set; }
    public float MaxHealth { get; private set; }
    public float MoveSpeed { get; private set; }
    public float AttackRange { get; private set; }
    public float AttackCooldown { get; private set; }

    public float CurrentHealth { get; private set; }

    private bool isDead;

    private void Awake()
    {
        if (data == null)
        {
            Debug.LogError($"EnemyStatsData is not assigned to {name}.", this);
            enabled = false;
            return;
        }

        Attack = data.startingAttack;
        Defense = data.startingDefense;
        MaxHealth = data.startingMaxHealth;
        MoveSpeed = data.startingMoveSpeed;
        AttackRange = data.startingAttackRange;
        AttackCooldown = data.startingAttackCooldown;

        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        float damage = Mathf.Max(amount - Defense, 0f);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0f);

        if (CurrentHealth <= 0f)
        {
            isDead = true;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{name} died", this);

        if (experienceGemPrefab != null)
        {
            ExperienceGem gem = Instantiate(
                experienceGemPrefab,
                transform.position,
                Quaternion.identity
            );

            gem.Initialize(data.experienceReward);
        }
        else
        {
            Debug.LogWarning($"ExperienceGem prefab is not assigned to {name}.", this);
        }

        Destroy(gameObject);
    }
}
