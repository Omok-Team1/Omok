using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReplayButtonController : MonoBehaviour
{
    [Header("텍스트 참조")]
    public TextMeshProUGUI replayNumberText;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI turnsText;

    [HideInInspector]
    public ReplayData associatedReplayData;

    public void SetReplayData(ReplayData replayData)
    {
        associatedReplayData = replayData;

        replayNumberText.text = $"Replay No.{replayData.ReplayNumber}";
        dateText.text = $"Date: {replayData.GameDate:yyyy-MM-dd HH:mm}";
        winnerText.text = $"Winner: {replayData.Winner}";
        turnsText.text = $"Turns: {replayData.TotalTurns}";
    }

    private void Start()
    {
        // 버튼 클릭 이벤트 설정
        Button buttonComponent = GetComponent<Button>();
        if (buttonComponent != null && associatedReplayData != null)
        {
            buttonComponent.onClick.AddListener(() => 
                ReplaySceneManager.LoadReplayScene(associatedReplayData));
        }
    }
}