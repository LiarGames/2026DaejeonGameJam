using UnityEngine;

[CreateAssetMenu(menuName = "Player/Stats Data")]
public class PlayerStatsData : ScriptableObject
{
    public float startingMaxHealth = 100f;
    public float startingAttack = 100f;
    public float startingMaxMana = 100f;
    public float startingDefense = 10f;
    public float startingHealthRegen = 1f;
    public float startingManaRegen = 10f;
    public float startingMoveSpeed = 4f;

    public float healthUpgrade = 100f;
    public float attackUpgrade = 20f;
    public float manaUpgrade = 50f;
    public float defenseUpgrade = 10f;
    public float healthRegenUpgrade = 1f;
    public float manaRegenUpgrade = 5f;
    public float moveSpeedUpgrade = 1f;
}