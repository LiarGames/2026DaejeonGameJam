using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Paused, LevelUp, GameOver }


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score")]
    [SerializeField] private float scorePerSecond = 10f;

    public GameState State { get; private set; }
    public int Score { get; private set; }
    public float SurvivalTime { get; private set; }

    public event Action<GameState> OnStateChanged;
    public event Action<int> OnScoreChanged;

    private float scoreAccumulator;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        SetState(GameState.Playing);
    }

    private void Update()
    {
        if (State != GameState.Playing)
            return;

        SurvivalTime += Time.deltaTime;

        // 생존 시간에 비례해 점수 누적.
        scoreAccumulator += scorePerSecond * Time.deltaTime;
        if (scoreAccumulator >= 1f)
        {
            int gained = Mathf.FloorToInt(scoreAccumulator);
            scoreAccumulator -= gained;
            AddScore(gained);
        }
    }

    public void AddScore(int amount)
    {
        Score += amount;
        OnScoreChanged?.Invoke(Score);
    }

    public void SetState(GameState next)
    {
        if (State == next)
            return;

        State = next;
        // GameOver/Paused면 게임 시간 정지.
        Time.timeScale = (next == GameState.Playing) ? 1f : 0f;
        OnStateChanged?.Invoke(next);
    }

    public void TogglePause()
    {
        // 게임오버·레벨업 선택 중엔 ESC 일시정지를 막는다.
        if (State == GameState.GameOver || State == GameState.LevelUp)
            return;

        SetState(State == GameState.Paused ? GameState.Playing : GameState.Paused);
    }

    // 레벨업 카드 선택 등으로 게임을 멈출 때. (일시정지 패널은 안 뜸)
    public void EnterLevelUp()
    {
        if (State == GameState.Playing)
            SetState(GameState.LevelUp);
    }

    // 카드 선택 후 게임 재개.
    public void ResumeFromLevelUp()
    {
        if (State == GameState.LevelUp)
            SetState(GameState.Playing);
    }

    public void GameOver()
    {
        SetState(GameState.GameOver);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        string current = SceneManager.GetActiveScene().name;

        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadScene(current);
        else
            SceneManager.LoadScene(current);
    }
}
