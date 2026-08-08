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
    public float ExperienceReward { get; private set; }
    public ExperienceGem ExperienceGemPrefab => experienceGemPrefab;

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
        ExperienceReward = data.experienceReward;
    }

    // 스포너가 스폰 직후(EnemyHealth.Start 이전) 호출해 웨이브 난이도 배율을 적용한다.
    public void ApplyScaling(float healthMultiplier, float speedMultiplier, float damageMultiplier)
    {
        MaxHealth *= healthMultiplier;
        MoveSpeed *= speedMultiplier;
        Attack *= damageMultiplier;
    }
}
