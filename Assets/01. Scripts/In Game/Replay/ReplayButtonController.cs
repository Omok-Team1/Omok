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
        dateText.text = $"{replayData.GameDate:yyyy-MM-dd HH:mm}";
        winnerText.text = replayData.Winner == Turn.PLAYER1 ? "흑 승" : 
            (replayData.Winner == Turn.PLAYER2 ? "백 승" : "무승부");
        turnsText.text = $"{replayData.TotalTurns}턴";
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