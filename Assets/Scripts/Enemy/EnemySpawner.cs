using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 등장 적 한 종류. 언제부터 나오는지와 등장 비중을 함께 정의한다.
[Serializable]
public class EnemyType
{
    public EnemyMovement prefab;

    [Tooltip("이 라운드부터 등장한다.")]
    [Min(1)] public int unlockRound = 1;

    [Tooltip("등장 비중. 클수록 자주 나온다.")]
    [Min(0f)] public float weight = 1f;
}

// 라운드마다 웨이브를 자동 생성해 스폰하고, 전멸시키면 다음 라운드로 넘어간다.
// 웨이브 구성은 표 없이 라운드 수에 따라 계산된다.
public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D player;
    [SerializeField] private EnemyType[] enemyTypes;

    [Header("Wave Size")]
    [Tooltip("1라운드 적 수")]
    [SerializeField] private int baseCount = 4;
    [Tooltip("라운드당 늘어나는 적 수")]
    [SerializeField] private float countPerRound = 6f;

    [Header("Difficulty Scaling (라운드당 복리)")]
    [Tooltip("플레이어 DPS가 스킬 수에 비례해 늘어나므로 체력도 복리로 올린다.")]
    [SerializeField] private float healthScalePerRound = 0.15f;
    [SerializeField] private float damageScalePerRound = 0.08f;
    [SerializeField] private float speedScalePerRound = 0.02f;
    [Tooltip("이동속도는 너무 오르면 회피가 불가능해지므로 상한을 둔다.")]
    [SerializeField] private float maxSpeedMultiplier = 1.6f;

    [Header("Pacing")]
    [SerializeField] private float spawnStagger = 0.15f;  // 웨이브 내 적 사이 스폰 간격
    [SerializeField] private float roundInterval = 2f;    // 라운드 사이 텀
    [SerializeField] private float roundTimeLimit = 90f;  // 전멸 못 시켜도 이 시간이면 다음 라운드

    [Header("Spawn Position")]
    [SerializeField] private float spawnRadius = 12f;     // 플레이어로부터의 스폰 거리

    // 라운드가 시작될 때 발생 (1부터). 배경 팝업 연출 등이 구독한다.
    public event Action<int> OnRoundStarted;

    public int CurrentRound { get; private set; }

    private readonly List<GameObject> _alive = new List<GameObject>();
    private readonly List<EnemyType> _unlocked = new List<EnemyType>();

    private void Start()
    {
        if (player == null)
            Debug.LogError("[Spawner] Player(Rigidbody2D)가 연결되지 않아 스폰되지 않습니다.", this);

        if (enemyTypes == null || enemyTypes.Length == 0)
        {
            Debug.LogError("[Spawner] Enemy Types가 비어 있어 스폰되지 않습니다.", this);
            return;
        }

        for (int i = 0; i < enemyTypes.Length; i++)
        {
            if (enemyTypes[i] == null || enemyTypes[i].prefab == null)
                Debug.LogWarning($"[Spawner] Enemy Types[{i}]에 프리팹이 없습니다.", this);
            else if (enemyTypes[i].prefab.GetComponent<EnemyStats>() == null)
                Debug.LogWarning($"[Spawner] '{enemyTypes[i].prefab.name}'에 EnemyStats가 없어 난이도 스케일링이 적용되지 않습니다.", this);
        }

        StartCoroutine(RunRounds());
    }

    private IEnumerator RunRounds()
    {
        CurrentRound = 0;

        while (true)
        {
            CurrentRound++;
            OnRoundStarted?.Invoke(CurrentRound);

            yield return StartCoroutine(SpawnWave(CurrentRound));

            // 전멸하거나 제한 시간이 지날 때까지 대기.
            float elapsed = 0f;
            while (CountAlive() > 0 && elapsed < roundTimeLimit)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(roundInterval);
        }
    }

    // --- 웨이브 생성 ---

    public int GetWaveCount(int round)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseCount + countPerRound * (round - 1)));
    }

    public float GetHealthMultiplier(int round) =>
        Mathf.Pow(1f + healthScalePerRound, round - 1);

    public float GetDamageMultiplier(int round) =>
        Mathf.Pow(1f + damageScalePerRound, round - 1);

    public float GetSpeedMultiplier(int round)
    {
        float value = Mathf.Pow(1f + speedScalePerRound, round - 1);

        // 0 이하면 상한을 두지 않은 것으로 본다. (0이면 적이 멈춰버리는 사고 방지)
        return maxSpeedMultiplier > 0f ? Mathf.Min(value, maxSpeedMultiplier) : value;
    }

    private void CollectUnlocked(int round)
    {
        _unlocked.Clear();

        foreach (EnemyType type in enemyTypes)
        {
            if (type != null && type.prefab != null && round >= type.unlockRound)
                _unlocked.Add(type);
        }
    }

    private IEnumerator SpawnWave(int round)
    {
        CollectUnlocked(round);
        if (_unlocked.Count == 0)
            yield break;

        int total = GetWaveCount(round);
        List<EnemyType> queue = new List<EnemyType>(total);

        // 해금된 종류는 최소 1마리씩 보장 (새로 열린 적이 안 나오는 일 방지).
        foreach (EnemyType type in _unlocked)
        {
            if (queue.Count < total)
                queue.Add(type);
        }

        // 나머지는 비중에 따라 랜덤 배정.
        while (queue.Count < total)
            queue.Add(PickWeighted());

        // 종류가 뭉쳐 나오지 않도록 순서를 섞는다.
        for (int i = queue.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (queue[i], queue[j]) = (queue[j], queue[i]);
        }

        foreach (EnemyType type in queue)
        {
            Spawn(type.prefab, round);

            if (spawnStagger > 0f)
                yield return new WaitForSeconds(spawnStagger);
        }
    }

    private EnemyType PickWeighted()
    {
        float totalWeight = 0f;
        foreach (EnemyType type in _unlocked)
            totalWeight += type.weight;

        if (totalWeight <= 0f)
            return _unlocked[UnityEngine.Random.Range(0, _unlocked.Count)];

        float roll = UnityEngine.Random.value * totalWeight;
        foreach (EnemyType type in _unlocked)
        {
            roll -= type.weight;
            if (roll <= 0f)
                return type;
        }

        return _unlocked[_unlocked.Count - 1];
    }

    private void Spawn(EnemyMovement prefab, int round)
    {
        if (prefab == null || player == null)
            return;

        // 플레이어를 중심으로 한 링 위 랜덤 지점.
        float angle = UnityEngine.Random.value * Mathf.PI * 2f;
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
        Vector2 spawnPos = player.position + offset;

        EnemyMovement enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

        // 스폰 직후 스탯에 라운드 배율 적용 (EnemyHealth.Start의 체력 초기화 이전).
        EnemyStats stats = enemy.GetComponent<EnemyStats>();
        if (stats != null)
        {
            stats.ApplyScaling(
                GetHealthMultiplier(round),
                GetSpeedMultiplier(round),
                GetDamageMultiplier(round)
            );
        }

        _alive.Add(enemy.gameObject);
    }

    // 파괴된 적을 정리하고 남은 수를 센다.
    private int CountAlive()
    {
        for (int i = _alive.Count - 1; i >= 0; i--)
        {
            if (_alive[i] == null)
                _alive.RemoveAt(i);
        }

        return _alive.Count;
    }

    // 밸런싱용: 인스펙터에서 컴포넌트 우클릭 → 라운드별 예상 수치를 출력한다.
    [ContextMenu("난이도 곡선 미리보기 (1~15라운드)")]
    private void PreviewDifficulty()
    {
        string report = "라운드 | 적 수 | 체력 | 공격력 | 속도 | 총 체력량\n";

        for (int round = 1; round <= 15; round++)
        {
            int count = GetWaveCount(round);
            float hp = GetHealthMultiplier(round);
            float pool = count * hp;

            report += $"{round,4} | {count,4} | {hp,5:F2}x | {GetDamageMultiplier(round),5:F2}x " +
                      $"| {GetSpeedMultiplier(round),4:F2}x | {pool,6:F1}\n";
        }

        Debug.Log(report, this);
    }
}
