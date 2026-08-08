using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;

    [Header("Health")]
    [SerializeField] private Image healthFill;      // Image Type = Filled
    [SerializeField] private TMP_Text healthText;


    private void Update()
    {
        if (stats == null)
            return;

        UpdateBar(healthFill, healthText, stats.CurrentHealth, stats.MaxHealth);
    }

    private void UpdateBar(Image fill, TMP_Text text, float current, float max)
    {
        if (fill != null)
            fill.fillAmount = max > 0f ? current / max : 0f;

        if (text != null)
            text.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }
}
