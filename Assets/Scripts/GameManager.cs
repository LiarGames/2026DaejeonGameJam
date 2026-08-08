using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Paused, GameOver }


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
        if (State == GameState.GameOver)
            return;

        SetState(State == GameState.Paused ? GameState.Playing : GameState.Paused);
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
