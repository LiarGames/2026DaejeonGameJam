using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;

    [Header("Health")]
    [SerializeField] private Image healthFill;      // Image Type = Filled
    [SerializeField] private TMP_Text healthText;

    [Header("Experience")]
    [SerializeField] private Image expFill;         // Image Type = Filled
    [SerializeField] private TMP_Text expText;
    [SerializeField] private TMP_Text levelText;

    [Header("Game")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;

    private void Update()
    {
        UpdatePlayerStats();
        UpdateGameStats();
    }

    private void UpdatePlayerStats()
    {
        if (stats == null)
            return;

        UpdateBar(healthFill, healthText, stats.CurrentHealth, stats.MaxHealth);
        UpdateBar(expFill, expText, stats.CurrentExperience, stats.ExperienceToLevelUp);

        if (levelText != null)
            levelText.text = $"Lv. {stats.Level}";
    }

    private void UpdateGameStats()
    {
        GameManager game = GameManager.Instance;
        if (game == null)
            return;

        if (scoreText != null)
            scoreText.text = $"{game.Score}";

        if (timeText != null)
        {
            int total = Mathf.FloorToInt(game.SurvivalTime);
            timeText.text = $"{total / 60:00}:{total % 60:00}";
        }
    }

    private void UpdateBar(Image fill, TMP_Text text, float current, float max)
    {
        if (fill != null)
            fill.fillAmount = max > 0f ? current / max : 0f;

        if (text != null)
            text.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }
}
