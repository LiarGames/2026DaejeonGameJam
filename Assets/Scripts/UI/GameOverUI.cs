using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIController : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        gameOverPanel.SetActive(false);

        restartButton.onClick.AddListener(Restart);
        quitButton.onClick.AddListener(Quit);
    }

    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += HandleGameStateChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state != GameState.GameOver)
            return;

        scoreText.text = $"{GameManager.Instance.Score:N0}";
        gameOverPanel.SetActive(true);
    }

    private void Restart()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Restart();
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}