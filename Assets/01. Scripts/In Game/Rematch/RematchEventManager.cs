using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; 

public class RematchEventManager : MonoBehaviour
{
    [SerializeField] private GameObject rematchEventPanel; // "잠깐!" + "상대 신청" 패널
    [SerializeField] private float eventChance = 0.5f;
    
    public UnityEvent OnEnemyRematchRequested = new UnityEvent();
    
    private void Start()
    {
        // 게임 종료 이벤트에 구독
        if (GameEndEventDispatcher.Instance != null)
        {
            // 먼저 이전 구독을 해제하여 중복 호출 방지
            GameEndEventDispatcher.Instance.OnGameEnded.RemoveListener(TryTriggerRematchEvent);
            // 다시 구독
            GameEndEventDispatcher.Instance.OnGameEnded.AddListener(TryTriggerRematchEvent);
            
            Debug.Log("GameEndEventDispatcher에 이벤트 리스너 등록됨");
        }
        else
        {
            Debug.LogError("GameEndEventDispatcher 인스턴스를 찾을 수 없습니다!");
        }
    }

    private void OnDestroy()
    {
        // 객체가 파괴될 때 이벤트 구독 해제
        if (GameEndEventDispatcher.Instance != null)
        {
            GameEndEventDispatcher.Instance.OnGameEnded.RemoveListener(TryTriggerRematchEvent);
            Debug.Log("RematchEventManager: OnDestroy에서 이벤트 리스너 해제됨");
        }
    }
    
    public void TryTriggerRematchEvent()
    {
        Debug.Log("TryTriggerRematchEvent 호출됨");
        float randomValue = Random.value;
        Debug.Log($"Random value: {randomValue}, Event chance: {eventChance}");
        
        if (randomValue <= eventChance)
        {
            Debug.Log("재대국 이벤트 활성화!");
            if (rematchEventPanel != null)
            {
                rematchEventPanel.SetActive(true);
                OnEnemyRematchRequested?.Invoke();
            }
            else
            {
                Debug.LogError("rematchEventPanel이 null입니다!");
            }
        }
    }

    // UI 버튼 이벤트용
    public void ConfirmRematchRequest()
    {
        if (rematchEventPanel != null)
        {
            rematchEventPanel.SetActive(false);
        }
        
        CurrentEnemyRematchButton rematchButton = FindObjectOfType<CurrentEnemyRematchButton>();
        if (rematchButton != null)
        {
            rematchButton.UnlockButton();
        }
        else
        {
            Debug.LogError("CurrentEnemyRematchButton을 찾을 수 없습니다!");
        }
    }
}