using UnityEngine;

// 플레이어 주변 링에 적을 계속 스폰하고, 경과 시간에 따라 난이도를 올린다.
public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D player;
    [SerializeField] private EnemyMovement[] enemyPrefabs; // 등장 가능한 적 종류

    [Header("Spawn Position")]
    [SerializeField] private float spawnRadius = 12f; // 플레이어로부터 이 거리(화면 밖)에 스폰

    [Header("Spawn Timing")]
    [SerializeField] private float baseInterval = 2f;         // 시작 스폰 간격(초)
    [SerializeField] private float minInterval = 0.3f;        // 간격 하한
    [SerializeField] private float intervalDecayPerMin = 1f;  // 분당 간격 감소량

    [Header("Difficulty Scaling (분 단위)")]
    [SerializeField] private float speedScalePerMin = 0.1f;   // 분당 이동/투사체 속도 +10%
    [SerializeField] private float damageScalePerMin = 0.15f; // 분당 공격력 +15%

    private float elapsed;
    private float spawnTimer;

    private void Update()
    {
        elapsed += Time.deltaTime;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            Spawn();
            spawnTimer = CurrentInterval();
        }
    }

    private float CurrentInterval()
    {
        float minutes = elapsed / 60f;
        return Mathf.Max(minInterval, baseInterval - intervalDecayPerMin * minutes);
    }

    private void Spawn()
    {
        if (player == null || enemyPrefabs == null || enemyPrefabs.Length == 0)
            return;

        EnemyMovement prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        // 플레이어를 중심으로 한 링 위 랜덤 지점.
        float angle = Random.value * Mathf.PI * 2f;
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
        Vector2 spawnPos = player.position + offset;

        EnemyMovement enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

        float minutes = elapsed / 60f;
        float speedMul = 1f + speedScalePerMin * minutes;
        float damageMul = 1f + damageScalePerMin * minutes;
        enemy.Initialize(player, speedMul, damageMul);
    }
}
