using UnityEngine;

// 버튼 OnClick이 SceneTransitionManager(싱글턴)를 호출하도록 중계한다.
// OnClick은 싱글턴 프로퍼티를 직접 못 부르므로 인스턴스 메서드로 감싼다.
public class SceneLoadButton : MonoBehaviour
{
    // 인스펙터에서 이동할 씬 이름을 넣어두고, OnClick은 인자 없는 Load()를 부르면 된다.
    [SerializeField] private string sceneName;

    public void Load()
    {
        LoadScene(sceneName);
    }

    // 버튼마다 다른 씬을 넘기고 싶으면 이 쪽(string 인자)을 OnClick에 연결해도 된다.
    public void LoadScene(string targetScene)
    {
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadScene(targetScene);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetScene); // 페이드 매니저가 없을 때 대비
    }
}
