using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    private float turnTimeLimit = 30f; // 30초 제한시간
    private float currentTurnTime = 0f;
    private bool isTurnRunning = false;
    private bool isPlayer1Turn = true; // 플레이어1이 턴을 시작하는지 여부

    // UI 요소
    public TextMeshProUGUI timerText;    // 텍스트 UI
    public Image FillImage;        // Radial fill 이미지

    private void Start()
    {
        StartTurn();
    }

    private void Update()
    {
        if (isTurnRunning)
        {
            currentTurnTime += Time.deltaTime;

            if (currentTurnTime >= turnTimeLimit)
            {
                // 제한시간 초과 시 상대 턴으로 넘어감
                Debug.Log((isPlayer1Turn ? "Player 1" : "Player 2") + " lost due to time limit.");

                // 제한시간 초과 시 이벤트 메시지를 큐에 추가
                EventManager.Instance.PublishMessageQueue();

                // 제한시간 초과 시 상대 턴으로 전환
                EndTurn(true);
            }

            // UI 업데이트
            UpdateUI();
        }
    }

    // 턴 시작
    private void StartTurn()
    {
        currentTurnTime = 0f;
        isTurnRunning = true;
        Debug.Log((isPlayer1Turn ? "Player 1" : "Player 2") + "'s turn started. Time limit: " + turnTimeLimit + " seconds.");
    }

    // 턴 종료
    private void EndTurn(bool timeExceeded = false)
    {
        if (timeExceeded)
        {
            // 시간 초과로 인해 상대 턴으로 전환
            Debug.Log("Turn ended due to time limit.");
        }

        // 턴이 끝나면 다른 플레이어의 턴으로 전환
        isPlayer1Turn = !isPlayer1Turn;
        StartTurn();
    }

    // 플레이어가 턴을 마쳤을 때 호출되는 함수
    public void EndPlayerTurn()
    {
        if (isTurnRunning)
        {
            isTurnRunning = false;
            EndTurn();
        }
    }

    // UI 업데이트
    private void UpdateUI()
    {
        // 남은 시간 계산
        float remainingTime = turnTimeLimit - currentTurnTime;
        
        // 남은 시간이 0일 때 0으로 표시하도록 수정
        timerText.text = Mathf.Floor(remainingTime).ToString("F0"); // 소수점 없이 표시
        
        // 남은 시간이 10초 이하일 때 색상 변경
        Color warningColor = new Color(0xEC / 255f, 0x27 / 255f, 0x27 / 255f); // #EC2727 색상

        if (remainingTime <= 11f)
        {
            // 타이머 텍스트 색상 변경
            timerText.color = warningColor;

            // Radial Fill 색상 변경
            FillImage.color = warningColor;
        }
        else
        {
            // 원래 색상으로 돌아가기 (타이머 텍스트와 FillImage의 기본 색상)
            timerText.color = Color.white;
            FillImage.color = Color.white;
        }

        // Radial Fill 업데이트
        FillImage.fillAmount = remainingTime / turnTimeLimit;
    }
}
