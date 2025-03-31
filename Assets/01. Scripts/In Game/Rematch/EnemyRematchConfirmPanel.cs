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
        yield return null;

        // GameResetManager를 통해 모든 DontDestroyOnLoad 객체 정리
        if (GameResetManager.Instance != null)
        {
            GameResetManager.Instance.CleanupDontDestroyObjects();
        }

        // 기존 초기화 로직
        if (EventManager.Instance != null)
        {
            EventManager.Instance.ClearEventQueue();
        }
    
        if (GameEndEventDispatcher.Instance != null)
        {
            GameEndEventDispatcher.Instance.ClearEventListeners();
            GameEndEventDispatcher.Instance.ResetDispatchState();
        }

        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(gameSceneName);
        gameObject.SetActive(false);
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }
}