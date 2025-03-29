using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class RandomRematchButton : MonoBehaviour
{
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private int requiredCoins = 100;
    [SerializeField] private string gameSceneName = "InGame"; 
    [SerializeField] private string titleSceneName = "Title";
    [SerializeField] private GameObject notEnoughCoinsPanel;

    private bool isButtonDisabled = false;

    public void OnClick()
    {
        if (isButtonDisabled)
            return;
            
        if (CoinManager.Instance.CheckCoin(requiredCoins))
        {
            if (confirmPanel != null)
            {
                confirmPanel.SetActive(true);
            }
        }
        else
        {
            CoinManager.Instance.CoinNotEnough();
            
            // 코인 부족시 알림 패널 표시
            if (notEnoughCoinsPanel != null)
            {
                notEnoughCoinsPanel.SetActive(true);
                // 버튼 비활성화
                isButtonDisabled = true;
            }
        }
    }

    public void Confirm()
{
    Debug.Log("리매치 확인 버튼 클릭됨");
    
    if (CoinManager.Instance != null && CoinManager.Instance.SpendCoin(requiredCoins))
    {
        Debug.Log($"코인 {requiredCoins}개 성공적으로 소모됨");
        
        // 이벤트 정리 - 인라인으로 처리
        if (GameEndEventDispatcher.Instance != null)
        {
            GameEndEventDispatcher.Instance.ClearEventListeners();
            GameEndEventDispatcher.Instance.ResetDispatchState();
        }
        
        if (EventManager.Instance != null)
        {
            // 이벤트 큐 클리어 메서드가 있다면 호출
            if (EventManager.Instance.GetType().GetMethod("ClearEventQueue") != null)
            {
                EventManager.Instance.ClearEventQueue();
            }
        }
        
        // 패널 닫기
        if (confirmPanel != null)
        {
            confirmPanel.SetActive(false);
        }
        
        // 직접 씬 로드 - 지연 없이
        Debug.Log($"게임 씬 직접 로드: {gameSceneName}");
        SceneManager.LoadScene(gameSceneName);
    }
    else
    {
        Debug.LogError("코인 소모 실패");
        
        if (confirmPanel != null)
        {
            confirmPanel.SetActive(false);
        }
    }
}

    private void CleanupAndLoadGameScene()
{
    Debug.Log("CleanupAndLoadGameScene 실행");
    
    // 모든 싱글톤 참조를 로컬 변수에 저장 (null 체크 및 예외 방지)
    var eventDispatcher = GameEndEventDispatcher.Instance;
    var eventManager = EventManager.Instance;
    
    // 이벤트 큐 초기화
    if (eventManager != null)
    {
        Debug.Log("이벤트 큐 초기화 시작");
        // 이벤트 큐 클리어
        eventManager.ClearEventQueue();
    }
    
    // 이벤트 리스너 초기화
    if (eventDispatcher != null)
    {
        Debug.Log("GameEndEventDispatcher 리스너 초기화");
        eventDispatcher.ClearEventListeners();
        eventDispatcher.ResetDispatchState();
    }
    
    // 씬 이름 로컬 변수로 복사
    string sceneName = gameSceneName;
    
    // 직접 씬 로드 - 즉시 실행
    Debug.Log($"게임 씬 즉시 로드 시도: {sceneName}");
    
    try
    {
        // 씬 로드를 직접 호출 - 코루틴 사용하지 않음
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        Debug.Log("씬 로드 명령 실행됨");
    }
    catch (System.Exception e)
    {
        Debug.LogError($"씬 로드 실패: {e.Message}");
        
        // 실패 시 비동기 로드 시도
        LoadSceneAsync(sceneName);
    }
}

	private void LoadSceneAsync(string sceneName)
{
    Debug.Log("비동기 씬 로드 시도");
    SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
}
    
    private IEnumerator FallbackLoadScene()
    {
        Debug.Log("백업 씬 로드 방식 시도");
        
        // 비동기 씬 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName);
        
        // 로드가 완료될 때까지 대기
        while (!asyncLoad.isDone)
        {
            Debug.Log($"씬 로드 진행률: {asyncLoad.progress * 100}%");
            yield return null;
        }
        
        Debug.Log("비동기 씬 로드 완료");
    }

private IEnumerator SafeSceneReload()
{
    // 현재 프레임의 모든 작업이 완료될 때까지 대기
    yield return null;
    
    // 조금 더 기다려서 이벤트 처리가 완료되도록 함
    yield return new WaitForSeconds(0.1f);
    
    Debug.Log($"게임 씬 이름: {gameSceneName} - 씬 로드 시도");
    
    try
    {
        // LoadSceneMode.Single로 씬을 완전히 새로 로드
        SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
        Debug.Log("씬 로드 요청 완료");
    }
    catch (System.Exception e)
    {
        Debug.LogError($"씬 로드 실패: {e.Message}");
        // 로드 실패 시 백업 방법으로 비동기 로드 시도
        StartCoroutine(FallbackLoadScene());
    }
}

    // 타이틀로 돌아가는 메소드
    public void ReturnToTitle()
    {
        // 게임 종료 이벤트 리스너 초기화 (중요)
        if (GameEndEventDispatcher.Instance != null)
        {
            GameEndEventDispatcher.Instance.ClearEventListeners();
            GameEndEventDispatcher.Instance.ResetDispatchState();
        }
        
        // 타이틀 씬으로 이동
        SceneManager.LoadScene(titleSceneName);
    }

    public void Cancel()
    {
        if (confirmPanel != null)
        {
            confirmPanel.SetActive(false);
        }
    }
    
    // 버튼 재활성화 메소드
    public void EnableButton()
    {
        isButtonDisabled = false;
    }
}