using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ReplaySceneManager : MonoBehaviour
{
    public static ReplayData CurrentReplayData { get; private set; }

    public static void LoadReplayScene(ReplayData replayData)
    {
        Debug.Log("[ReplaySceneManager] LoadReplayScene called");
        Debug.Log($"[ReplaySceneManager] Replay Data: Moves Count = {replayData?.GameMoves?.Count ?? 0}");
        
        CurrentReplayData = replayData;
        SceneManager.LoadScene("ReplayScene");
    }

    void Start()
{
    StartCoroutine(InitializeAfterSceneLoad());
}

private IEnumerator InitializeAfterSceneLoad()
{
    yield return null; // 씬 로드 완료 대기
    var viewController = FindObjectOfType<ReplayViewController>();
    if (viewController != null && CurrentReplayData != null)
    {
        viewController.Initialize(CurrentReplayData);
    }
}

    public static void UnloadReplayScene()
    {
        
        SceneManager.LoadScene("Title");
    }
}