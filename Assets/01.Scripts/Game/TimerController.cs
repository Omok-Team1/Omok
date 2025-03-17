using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    public enum PlayerState { Playing, Win, Lose }
    public PlayerState player1State = PlayerState.Playing;
    public PlayerState player2State = PlayerState.Playing;

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
                // 제한시간 초과 시 상태를 Lose로 변경
                if (isPlayer1Turn)
                {
                    player1State = PlayerState.Lose;
                    Debug.Log("Player 1 lost due to time limit.");
                }
                else
                {
                    player2State = PlayerState.Lose;
                    Debug.Log("Player 2 lost due to time limit.");
                }

                // 제한시간 초과 시 이벤트 메시지를 큐에 추가
                EventManager.Instance.PublishMessageQueue();

                isTurnRunning = false;
                EndTurn();
            }

            // UI 업데이트
            UpdateUI();
        }

        // 플레이어의 상태가 Lose로 변경되면 다른 플레이어는 Win 상태가 됨
        CheckForWinner();
    }

    // 턴 시작
    private void StartTurn()
    {
        currentTurnTime = 0f;
        isTurnRunning = true;
        if (isPlayer1Turn)
        {
            player1State = PlayerState.Playing;
            Debug.Log("Player 1's turn started. Time limit: " + turnTimeLimit + " seconds.");
        }
        else
        {
            player2State = PlayerState.Playing;
            Debug.Log("Player 2's turn started. Time limit: " + turnTimeLimit + " seconds.");
        }
    }

    // 턴 종료
    private void EndTurn()
    {
        // 턴이 끝나면 다른 플레이어의 턴으로 전환
        isPlayer1Turn = !isPlayer1Turn;
        StartTurn();
    }

    // 플레이어의 상태가 Lose일 때 다른 플레이어를 Win으로 설정
    private void CheckForWinner()
    {
        if (player1State == PlayerState.Lose && player2State == PlayerState.Playing)
        {
            player2State = PlayerState.Win;
            Debug.Log("Player 2 wins!");
        }
        else if (player2State == PlayerState.Lose && player1State == PlayerState.Playing)
        {
            player1State = PlayerState.Win;
            Debug.Log("Player 1 wins!");
        }
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
        timerText.text = Mathf.Ceil(remainingTime).ToString("F0"); // 소수점 없이 표시

        // Radial Fill 업데이트
        FillImage.fillAmount = remainingTime / turnTimeLimit;
    }
}