using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TimerController2 : MonoBehaviour
{
    private float turnTimeLimit = 30f; // 30초 제한시간
    private float currentTurnTime = 0f;
    private bool isTurnRunning = false;
    private bool isPlayer1Turn = true; // 플레이어1이 턴을 시작하는지 여부

    public TextMeshProUGUI timerText;    // 시간 표시를 위한 텍스트 UI
    public Image FillImage;              // Horizontal Fill 이미지를 사용한 타이머 진행률
    public AudioSource countdownAudioSource; // 카운트다운 사운드를 위한 AudioSource
    public AudioClip countdownSound;         // 카운트다운 소리 (오디오 클립)

    private bool hasPlayedCountdownSound = false; // 10초에서 한 번만 카운트다운 사운드를 재생하기 위한 변수

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
            
            if (currentTurnTime >= turnTimeLimit - 8f && !hasPlayedCountdownSound)
            {
                PlayCountdownSound();
                hasPlayedCountdownSound = true; // 사운드를 한 번만 재생하도록 설정
            }
        }
    }

    private void PlayCountdownSound()
    {
        if (countdownAudioSource != null && countdownSound != null)
        {
            countdownAudioSource.PlayOneShot(countdownSound); // 카운트다운 소리를 한 번 재생
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

        // 유예시간 추가 (예: 1초)
        StartCoroutine(WaitForNextTurn(1f)); // 1초 유예시간 추가
    }

    // 코루틴을 사용하여 유예시간 후 턴을 넘기는 함수
    private IEnumerator WaitForNextTurn(float delayTime)
    {
        // 유예시간 동안 기다림
        yield return new WaitForSeconds(delayTime);

        // 턴을 넘기고 다음 플레이어의 턴 시작
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
        
        timerText.text = Mathf.Floor(remainingTime * 100) / 100f == 0 ? "00.00" : (Mathf.Floor(remainingTime * 100) / 100f).ToString("00.00");

        // 남은 시간이 0일 때 0으로 표시하도록 수정
        timerText.text = remainingTime.ToString("00.00") + " sec"; // 00.00 형식으로 표시
        
        // 남은 시간이 10초 이하일 때 색상 변경
        Color warningColor = new Color(0xEC / 255f, 0x27 / 255f, 0x27 / 255f); // #EC2727 색상

        if (remainingTime <= 10f)
        {
            // 타이머 텍스트 색상 변경
            timerText.color = warningColor;

            // FillImage 색상 변경
            FillImage.color = warningColor;
        }
        else
        {
            // 원래 색상으로 돌아가기 (타이머 텍스트와 FillImage의 기본 색상)
            timerText.color = Color.white;
            FillImage.color = Color.white;
        }

        // FillImage 업데이트
        FillImage.fillAmount = remainingTime / turnTimeLimit; // Horizontal Fill 방식으로 타이머 진행률 표시
    }
}
