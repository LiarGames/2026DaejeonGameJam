using System.Collections;
using UnityEngine;

// 튜토리얼에서 레벨업 카드 선택창을 원하는 타이밍에 띄운다.
// 실제 경험치와 무관하게 PlayerStats.LevelUp()을 직접 호출한다.
public class TutorialLevelUpTrigger : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    [Header("자동 실행")]
    [Tooltip("켜면 씬 시작 후 지정한 시간이 지나 자동으로 카드창을 띄운다.")]
    [SerializeField] private bool triggerOnStart = true;
    [Min(0f)]
    [SerializeField] private float delay = 1.5f;

    [Tooltip("한 번만 발생시킨다.")]
    [SerializeField] private bool onlyOnce = true;

    private bool _triggered;

    private void Start()
    {
        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogWarning("[Tutorial] 씬에서 PlayerStats를 찾지 못했습니다.", this);
            return;
        }

        if (triggerOnStart)
            StartCoroutine(TriggerAfterDelay());
    }

    private IEnumerator TriggerAfterDelay()
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        TriggerLevelUp();
    }

    // 버튼 OnClick 등에서 직접 호출할 수도 있다.
    public void TriggerLevelUp()
    {
        if (playerStats == null)
            return;

        if (onlyOnce && _triggered)
            return;

        _triggered = true;
        playerStats.LevelUp();
    }
}
