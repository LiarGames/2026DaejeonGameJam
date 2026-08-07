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


public class PlayerStats : MonoBehaviour
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
    }

    private void Update()
    {
        RegenerateHealth();
        RegenerateMana(); 
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