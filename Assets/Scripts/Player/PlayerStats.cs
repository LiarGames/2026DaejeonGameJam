using System;
using UnityEngine;

public enum StatType
{
    MaxHealth,
    Attack,
    MaxMana,
    Defense,
    HealthRegen,
    ManaRegen,
    MoveSpeed
}


public class PlayerStats : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerStatsData data;

    public float Attack { get; private set; }
    public float Defense { get; private set; }
    public float MaxHealth { get; private set; }
    public float MaxMana { get; private set; }
    public float HealthRegen { get; private set; }
    public float ManaRegen { get; private set; }
    public float MoveSpeed { get; private set; }

    public float CurrentHealth { get; private set; }
    public float CurrentMana { get; private set; }

    public int Level { get; private set; } = 1;
    public float CurrentExperience { get; private set; }
    public float ExperienceToLevelUp { get; private set; } = 100f;

    // 레벨업 시 발생. LevelUpController가 구독해 카드 선택을 띄운다.
    public event Action OnLevelUp;

    private void Awake()
    {   
        Attack = data.startingAttack;
        Defense = data.startingDefense;
        MaxHealth = data.startingMaxHealth;
        MaxMana = data.startingMaxMana;
        HealthRegen = data.startingHealthRegen;
        ManaRegen = data.startingManaRegen;
        MoveSpeed = data.startingMoveSpeed;

        CurrentHealth = MaxHealth;
        CurrentMana = MaxMana;

        Level = 1;
        CurrentExperience = 0f;
        ExperienceToLevelUp = 100f;
    }

    private void Update()
    {
        RegenerateHealth();
        RegenerateMana(); 
    }

    public void GainExperience(float amount)
    {
        if (amount <= 0f)
            return;

        CurrentExperience += amount;

        while (CurrentExperience >= ExperienceToLevelUp)
        {
            CurrentExperience -= ExperienceToLevelUp;
            LevelUp();
        }
    }

    public void LevelUp()
    {
        Level++;
        ExperienceToLevelUp += 20f;

        OnLevelUp?.Invoke();
    }

    public void TakeDamage(float amount)
    {
        float damage = Mathf.Max(amount - Defense, 0f);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0f);

        if (CurrentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
    }

    public void UpgradeStat(StatType stat)
    {
        switch (stat)
        {
            case StatType.MaxHealth:
                MaxHealth += data.healthUpgrade;
                CurrentHealth += data.healthUpgrade;
                break;

            case StatType.Attack:
                Attack += data.attackUpgrade;
                break;

            case StatType.MaxMana:
                MaxMana += data.manaUpgrade;
                CurrentMana += data.manaUpgrade;
                break;

            case StatType.Defense:
                Defense += data.defenseUpgrade;
                break;

            case StatType.HealthRegen:
                HealthRegen += data.healthRegenUpgrade;
                break;

            case StatType.ManaRegen:
                ManaRegen += data.manaRegenUpgrade;
                break;

            case StatType.MoveSpeed:
                MoveSpeed += data.moveSpeedUpgrade;
                break;
        }
    }

    private void RegenerateHealth()
    {
        CurrentHealth = Mathf.Min(
            CurrentHealth + HealthRegen * Time.deltaTime,
            MaxHealth
        );
    }

    private void RegenerateMana()
    {
        CurrentMana = Mathf.Min(
            CurrentMana + ManaRegen * Time.deltaTime,
            MaxMana
        );
    }
}
