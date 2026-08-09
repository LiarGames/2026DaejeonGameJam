using System.Collections;
using UnityEngine;

// 씬이 시작될 때 암전에서 서서히 밝아진다.
// 씬 전환 매니저를 거치지 않고 직접 Play해도 동일하게 동작한다.
[RequireComponent(typeof(CanvasGroup))]
public class SceneFadeIn : MonoBehaviour
{
    [SerializeField] private float duration = 0.6f;
    [Tooltip("밝아지기 전에 검은 화면을 유지할 시간")]
    [Min(0f)]
    [SerializeField] private float holdBefore = 0.1f;

    private CanvasGroup _group;

    private void Awake()
    {
        _group = GetComponent<CanvasGroup>();

        // 첫 프레임부터 가려지도록 즉시 암전.
        _group.alpha = 1f;
        _group.blocksRaycasts = true;
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        if (holdBefore > 0f)
            yield return new WaitForSecondsRealtime(holdBefore);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            // timeScale에 영향받지 않도록 unscaled 사용.
            elapsed += Time.unscaledDeltaTime;
            _group.alpha = Mathf.Clamp01(1f - elapsed / duration);
            yield return null;
        }

        _group.alpha = 0f;
        _group.blocksRaycasts = false;
    }
}
