using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TimerController : MonoBehaviour
{
    private float turnTimeLimit = 30f; // 30초 제한시간
    
    private float currentTurnTime = 0f;
    private bool isTurnRunning = false;
    private bool isPlayer1Turn = true; // 플레이어1이 턴을 시작하는지 여부

    public TextMeshProUGUI timerText;    // 텍스트 UI
    public Image FillImage;        // Radial fill 이미지
    
    public AudioSource warningAudioSource; // 경고 사운드를 위한 AudioSource
    public AudioClip warningSound;         // 경고 사운드 (오디오 클립)

    private bool hasPlayedWarningSound = false; // 경고 사운드를 한 번만 재생하도록 설정

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
                EventMessage message;
                
                if (isPlayer1Turn is false)
                    message = new EventMessage("Player2TimeOver");
                else
                    message = new EventMessage("Player1TimeOver");
                
                EventManager.Instance.PushEventMessageEvent(message);
                
                // 제한시간 초과 시 상대 턴으로 전환
                EndTurn(true);
            }
            
            PlayWarningSound();

            // UI 업데이트
            UpdateUI();
        }
    }
    
    // 경고 사운드를 처리하는 함수
    private void PlayWarningSound()
    {
        float remainingTime = turnTimeLimit - currentTurnTime;

        if (remainingTime <= 8f && !hasPlayedWarningSound)
        {
            // 8초일 때 경고 사운드를 한 번만 재생
            if (warningAudioSource != null && warningSound != null)
            {
                warningAudioSource.PlayOneShot(warningSound); // 경고 소리 재생
            }
            hasPlayedWarningSound = true; // 사운드를 한 번만 재생하도록 설정
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
            // 사운드 멈추기
            if (warningAudioSource.isPlaying)
            {
                warningAudioSource.Stop(); // 사운드 멈추기
            }

            // 사운드 초기화
            hasPlayedWarningSound = false;
            
            isTurnRunning = false;
            EndTurn();
        }
    }

    // UI 업데이트
    private void UpdateUI()
    {
        // 남은 시간 계산
        float remainingTime = turnTimeLimit - currentTurnTime;
        
        remainingTime = Mathf.Max(remainingTime, 0f);
        
        // 남은 시간이 0일 때 0으로 표시하도록 수정
        timerText.text = Mathf.Floor(remainingTime).ToString("F0"); // 소수점 없이 표시
        
        // 남은 시간이 10초 이하일 때 색상 변경
        Color warningColor = new Color(0xEC / 255f, 0x27 / 255f, 0x27 / 255f); // #EC2727 색상

        if (remainingTime <= 10f)
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
            FillImage.color = new Color(0xE6 / 255f, 0xE6 / 255f, 0xE6 / 255f);
        }

        // Radial Fill 업데이트
        FillImage.fillAmount = remainingTime / turnTimeLimit;
    }
}