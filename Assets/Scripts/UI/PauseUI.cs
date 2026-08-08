using UnityEngine;

// 게임 상태를 구독해서 일시정지 패널을 자동으로 켜고 끈다.
// GameManager는 UI를 몰라도 되고, 이 스크립트만 상태를 반영한다.
public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Start 시점엔 모든 Awake가 끝나 GameManager.Instance가 준비돼 있다.
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += HandleStateChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState state)
    {
        if (pausePanel != null)
            pausePanel.SetActive(state == GameState.Paused);
    }
}
