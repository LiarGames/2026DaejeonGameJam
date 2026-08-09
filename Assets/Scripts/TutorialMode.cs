using UnityEngine;

// 튜토리얼(HowToPlay) 씬에 두는 스위치.
// 시연은 그대로 보여주되 실제 적용은 막는다. 항목별로 켜고 끌 수 있다.
public class TutorialMode : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("무엇을 막을지")]
    [Tooltip("플레이어 이동을 막는다.")]
    [SerializeField] private bool disableMovement = true;

    [Tooltip("체력이 깎이지 않는다. (사망·게임오버도 발생하지 않음)")]
    [SerializeField] private bool disableDamage = true;

    [Tooltip("경험치를 얻지 않는다. (레벨업·카드 선택도 뜨지 않음)")]
    [SerializeField] private bool disableExperience = true;

    private void Start()
    {
        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();

        if (playerStats != null)
        {
            playerStats.Invulnerable = disableDamage;
            playerStats.ExperienceEnabled = !disableExperience;
        }
        else
        {
            Debug.LogWarning("[Tutorial] 씬에서 PlayerStats를 찾지 못했습니다.", this);
        }

        if (!disableMovement)
            return;

        if (playerMovement == null && playerStats != null)
            playerMovement = playerStats.GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.StopMovement();
            playerMovement.enabled = false;
        }
    }
}
