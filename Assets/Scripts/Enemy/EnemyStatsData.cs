using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Stats Data")]
public class EnemyStatsData : ScriptableObject
{
    public float startingMaxHealth = 100f;
    public float startingAttack = 10f;
    public float startingDefense = 0f;
    public float startingMoveSpeed = 5f;
    public float startingAttackRange = 5f;
    public float startingAttackCooldown = 1f;
}
