using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ReplayListUIController : MonoBehaviour
{
    [Header("Replay Button Prefab")]
    [SerializeField] private GameObject replayButtonPrefab;

    [Header("UI References")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private ScrollRect scrollRect;

    private void Start()
    {
        Debug.Log("ReplayListUIController Starting...");
        PopulateReplayList();
    }

    private void PopulateReplayList()
    {
        // 기존 자식 오브젝트 제거
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    
        var replays = ReplayManager.Instance.GetReplays();
    
        Debug.Log($"Total Replays to Display: {replays.Count}");
    
        foreach (var replay in replays)
        {
            // 전체 프리팹 인스턴스화 (버튼과 텍스트 전체 구조 유지)
            GameObject replayButton = Instantiate(replayButtonPrefab, contentParent);
            
            // 텍스트 컴포넌트 찾기 (Explicit references)
            ReplayButtonController replayButtonController = replayButton.GetComponent<ReplayButtonController>();
            
            if (replayButtonController != null)
            {
                // Use the existing SetReplayData method
                replayButtonController.SetReplayData(replay);
                
                Debug.Log($"Created Replay Button - No.{replay.ReplayNumber}, " +
                          $"Date: {replay.GameDate}, " +
                          $"Winner: {replay.Winner}, " +
                          $"Turns: {replay.TotalTurns}");
            }
            else
            {
                Debug.LogError("ReplayButtonController not found on instantiated prefab!");
            }
        }
    
        // 강제 캔버스 업데이트
        Canvas.ForceUpdateCanvases();
    }
}