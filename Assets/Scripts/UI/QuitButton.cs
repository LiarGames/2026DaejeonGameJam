using UnityEngine;

// 게임 종료 버튼용. OnClick은 정적 메서드를 직접 부를 수 없어 인스턴스 메서드로 감싼다.
public class QuitButton : MonoBehaviour
{
    public void Quit()
    {
#if UNITY_EDITOR
        // 에디터에서는 Application.Quit()이 동작하지 않으므로 플레이 모드를 끈다.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
