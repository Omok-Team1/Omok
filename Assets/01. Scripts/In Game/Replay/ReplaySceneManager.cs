using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplaySceneManager : MonoBehaviour
{
    public static ReplayData CurrentReplayData { get; private set; }

    public static void LoadReplayScene(ReplayData replayData)
    {
        CurrentReplayData = replayData;
        SceneManager.LoadScene("ReplayScene");
    }

    void Start()
    {
        var viewController = FindObjectOfType<ReplayViewController>();
        if (viewController != null && ReplaySceneManager.CurrentReplayData != null)
        {
            viewController.Initialize(ReplaySceneManager.CurrentReplayData);
        }
        else
        {
            Debug.LogError("ViewController 또는 ReplayData가 없습니다!");
        }
    }
}