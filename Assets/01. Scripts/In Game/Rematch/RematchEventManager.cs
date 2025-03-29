using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; 
public class RematchEventManager : MonoBehaviour
{
    [SerializeField] private GameObject rematchEventPanel; // "잠깐!" + "상대 신청" 패널
    [SerializeField] private float eventChance = 0.5f;
    
    public UnityEvent OnEnemyRematchRequested;
    
    private void OnEnable()
    {
        if (GameEndEventDispatcher.Instance != null)
        {
            GameEndEventDispatcher.Instance.OnGameEnded.AddListener(TryTriggerRematchEvent);
        }
        else
        {
            Debug.LogError("GameEndEventDispatcher 인스턴스를 찾을 수 없습니다!");
        }
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        GameEndEventDispatcher.Instance.OnGameEnded.RemoveListener(TryTriggerRematchEvent);
    }
    

    public void TryTriggerRematchEvent()
    {
        if (Random.value <= eventChance)
        {
            rematchEventPanel.SetActive(true);
            OnEnemyRematchRequested?.Invoke();
        }
    }

    // UI 버튼 이벤트용
    public void ConfirmRematchRequest()
    {
        rematchEventPanel.SetActive(false);
        FindObjectOfType<CurrentEnemyRematchButton>().UnlockButton();
    }
}