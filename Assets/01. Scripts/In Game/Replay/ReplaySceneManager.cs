using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplaySceneManager : MonoBehaviour
{

    private void Start()
    {
        GetComponent<ReplaySceneManager>().InitializeReplayScene();
    }
    
    // 씬 간 데이터 전달을 위한 static 변수
    public static ReplayData CurrentReplayData { get; private set; }

    // 리플레이 씬으로 전환하는 메서드 (ReplayListUIController에서 호출)
    public static void LoadReplayScene(ReplayData replayData)
    {
        CurrentReplayData = replayData;
        SceneManager.LoadScene("ReplayScene");
    }

    // ReplayViewController에서 씬 로드 시 호출
    public void InitializeReplayScene()
    {
        if (CurrentReplayData != null)
        {
            ReplayViewController replayController = FindObjectOfType<ReplayViewController>();
            if (replayController != null)
            {
                replayController.LoadReplay(CurrentReplayData);
            }
        }
    }
}