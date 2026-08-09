using UnityEngine;

// UI 요소를 천천히 점멸시켜 시선을 끈다. (튜토리얼 Next/Back 버튼 등)
// CanvasGroup 알파를 조절하므로 버튼과 그 안의 텍스트가 함께 깜빡인다.
[RequireComponent(typeof(CanvasGroup))]
public class UIPulse : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float minAlpha = 0.35f;
    [Range(0f, 1f)]
    [SerializeField] private float maxAlpha = 1f;

    [Tooltip("한 번 밝아졌다 어두워지는 데 걸리는 시간(초)")]
    [SerializeField] private float cycleDuration = 1.6f;

    private CanvasGroup _group;
    private float _time;

    private void Awake()
    {
        _group = GetComponent<CanvasGroup>();
    }

    private void OnDisable()
    {
        // 꺼질 때 원래 밝기로 되돌린다.
        if (_group != null)
            _group.alpha = maxAlpha;
    }

    private void Update()
    {
        if (_group == null || cycleDuration <= 0f)
            return;

        // 일시정지(timeScale 0) 중에도 점멸이 이어지도록 unscaled 사용.
        _time += Time.unscaledDeltaTime;

        float wave = (Mathf.Sin(_time / cycleDuration * Mathf.PI * 2f) + 1f) * 0.5f;
        _group.alpha = Mathf.Lerp(minAlpha, maxAlpha, wave);
    }
}
