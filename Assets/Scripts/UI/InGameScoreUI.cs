using TMPro;
using UnityEngine;

public class InGameScoreUI : MonoBehaviour
{
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private string scorePrefix = "Score: ";

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.Instance;

        if (_gameManager == null)
        {
            Debug.LogError("InGameScoreUI could not find GameManager.", this);
            return;
        }

        _gameManager.OnScoreChanged += UpdateScore;
        _gameManager.OnStateChanged += HandleGameStateChanged;

        UpdateScore(_gameManager.Score);
        HandleGameStateChanged(_gameManager.State);
    }

    private void OnDestroy()
    {
        if (_gameManager == null)
            return;

        _gameManager.OnScoreChanged -= UpdateScore;
        _gameManager.OnStateChanged -= HandleGameStateChanged;
    }

    private void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"{scorePrefix}{score:N0}";
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (scorePanel != null)
            scorePanel.SetActive(state != GameState.GameOver);
    }
}
