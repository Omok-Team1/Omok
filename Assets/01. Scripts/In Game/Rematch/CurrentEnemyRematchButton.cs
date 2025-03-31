using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class CurrentEnemyRematchButton : MonoBehaviour
{
    [SerializeField] private Button currentEnemyRematchButton;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private float unlockAnimDuration = 0.3f;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private int requiredCoins = 0;
    [SerializeField] private string gameSceneName = "InGame";
    [SerializeField] private string titleSceneName = "Title";
    [SerializeField] private GameObject notEnoughCoinsPanel;
    
    // 초기화할 GameObject 목록 (인스펙터에서 설정)
    [Header("이벤트 리스너 초기화 설정")]
    [SerializeField] private List<GameObject> listenerObjectsToClear;
    [SerializeField] private List<string> listenerEventsToClear;
    [SerializeField] private bool clearAllListeners = false; // 전체 초기화 여부 (주의: 모든 리스너 삭제됨)

    private void Awake()
    {
        // 초기 상태: 잠금
        LockButton();
    }

    public void UnlockButton()
    {
        // 애니메이션과 함께 잠금 해제
        lockOverlay.transform.DOScale(1.2f, unlockAnimDuration)
            .OnComplete(() => 
            {
                lockOverlay.SetActive(false);
                currentEnemyRematchButton.interactable = true;
                transform.DOScale(1f, unlockAnimDuration);
            });
    }


    public void OnRematchButtonClicked()
    {
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
        
            }
        }
    }
    
    public void Confirm()
    {
        if (CoinManager.Instance != null && CoinManager.Instance.SpendCoin(requiredCoins))
        {
            // 1. 패널 비활성화 (버튼 클릭 직후)
            if (confirmPanel != null)
            {
                confirmPanel.SetActive(false);
            }

            // 2. 코루틴을 GameManager에서 실행 (활성화된 객체)
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartCoroutine(SafeSceneReload());
            }
            else
            {
                // GameManager가 없을 경우 직접 실행 (비권장)
                StartCoroutine(SafeSceneReload());
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

        // 이벤트 리스너 초기화 - 선택적 초기화 방식으로 변경
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
        // 1. 패널 닫기
        if (confirmPanel != null) confirmPanel.SetActive(false);

        // 2. 이벤트 시스템 초기화 (선택적 초기화 방식으로 변경)
        if (EventManager.Instance != null)
        {
            EventManager.Instance.ClearEventQueue();
            
            // 설정에 따라 선택적으로 리스너 초기화
            if (clearAllListeners)
            {
                // 기존 방식 (전체 초기화) - 주의: 모든 리스너 삭제됨
                EventManager.Instance.ClearAllListeners();
            }
            else
            {
                // 객체별 초기화
                if (listenerObjectsToClear != null && listenerObjectsToClear.Count > 0)
                {
                    EventManager.Instance.ClearListenersForGameObjects(listenerObjectsToClear);
                }
                
                // 이벤트별 초기화
                if (listenerEventsToClear != null && listenerEventsToClear.Count > 0)
                {
                    EventManager.Instance.ClearListenersForEvents(listenerEventsToClear);
                }
            }
        }

        if (GameEndEventDispatcher.Instance != null)
        {
            GameEndEventDispatcher.Instance.ClearEventListeners();
        }

        // 3. DontDestroyOnLoad 객체 정리 (GameResetManager 사용)
        if (GameResetManager.Instance != null)
        {
            GameResetManager.Instance.CleanupDontDestroyObjects();
        }

        // 4. 짧은 지연 후 씬 로드
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(gameSceneName);
    }
    
    public void Cancel()
    {
        if (confirmPanel != null)
        {
            confirmPanel.SetActive(false);
        }
    }
    
    
    public void LockButton()
    {
        lockOverlay.SetActive(true);
        currentEnemyRematchButton.interactable = false;
    }
}