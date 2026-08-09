using UnityEngine;

// 버튼 OnClick이 GameManager(싱글턴)를 호출하도록 중계한다.
// 프리팹 안의 버튼은 씬 오브젝트를 직접 참조할 수 없으므로 런타임에 Instance를 찾는다.
public class GameManagerButton : MonoBehaviour
{
    public void TogglePause()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.TogglePause();
    }

    public void Restart()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Restart();
    }

    public void GameOver()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
    }
}
