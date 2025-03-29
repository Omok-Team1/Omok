using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
public class EnemyRematchConfirmPanel : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "InGame";
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void ConfirmRematch()
    {
        // 이벤트 처리 전 정리 작업
        StartCoroutine(SafeRestart());
    }
    
    private IEnumerator SafeRestart()
    {
        // 현재 프레임의 모든 작업이 완료될 때까지 대기
        yield return null;
    
        // 씬 로드 전 모든 이벤트 및 리스너 초기화
        if (EventManager.Instance != null)
        {
            EventManager.Instance.ClearEventQueue();
        }
    
        if (GameEndEventDispatcher.Instance != null)
        {
            GameEndEventDispatcher.Instance.ClearEventListeners();
            GameEndEventDispatcher.Instance.ResetDispatchState();
        }
    
        // 짧은 지연 시간
        yield return new WaitForSeconds(0.1f);
    
        // 코인 소모 없이 씬 재시작
        SceneManager.LoadScene(gameSceneName);
        gameObject.SetActive(false);
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }
}