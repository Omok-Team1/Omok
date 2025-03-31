using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void OnQuitButtonClicked()
    {
        // 에디터에서 실행 중일 때는 플레이 모드 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        // 빌드된 게임에서는 게임 종료
#else
        Application.Quit();
#endif
    }
}