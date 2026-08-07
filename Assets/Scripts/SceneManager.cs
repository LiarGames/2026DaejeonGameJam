using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneTransitionManager : MonoBehaviour
{
    
    public static SceneTransitionManager Instance { get; private set; }

    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeDuration = 0.4f;

    private bool _isLoading;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        if (_isLoading) return;
        StartCoroutine(LoadRoutine(sceneName));
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        _isLoading = true;

        yield return StartCoroutine(Fade(1f));          // 페이드 아웃

        var op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f) yield return null;    // 로딩 대기
        op.allowSceneActivation = true;
        while (!op.isDone) yield return null;

        yield return StartCoroutine(Fade(0f));          // 페이드 인

        _isLoading = false;
    }

    private IEnumerator Fade(float target)
    {
        fadeCanvas.blocksRaycasts = true;
        float start = fadeCanvas.alpha, t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            fadeCanvas.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        fadeCanvas.alpha = target;
        fadeCanvas.blocksRaycasts = target > 0.5f;
    }
}

