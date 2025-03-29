using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameEndEventDispatcher : MonoBehaviour
{
    public static GameEndEventDispatcher Instance { get; private set; }
    public UnityEvent OnGameEnded = new UnityEvent();

    private bool hasDispatchedEvent = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환시에도 유지
        }
        else
        {
            // 이미 인스턴스가 있다면, 이 객체는 파괴하고 기존 인스턴스 이벤트 상태 초기화
            Instance.ClearEventListeners();
            Instance.ResetDispatchState();
            Destroy(gameObject);
            return;
        }
    }
    
    // 씬이 로드될 때 이벤트 리스너 초기화
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 모든 씬이 로드될 때마다 이벤트 초기화 및 상태 리셋
        ClearEventListeners();
        ResetDispatchState();
        Debug.Log($"씬 {scene.name} 로드됨: 이벤트 리스너 및 상태 초기화");
    }
    
    // 모든 이벤트 리스너 초기화
    public void ClearEventListeners()
    {
        Debug.Log("이벤트 리스너 초기화됨");
        OnGameEnded.RemoveAllListeners();
    }
    
    // 이벤트 디스패치 상태 초기화
    public void ResetDispatchState()
    {
        hasDispatchedEvent = false;
        Debug.Log("이벤트 디스패치 상태 초기화됨");
    }
    
    // 이벤트 발생 함수
    public void DispatchGameEndEvent()
    {
        // 중복 호출 방지
        if (!hasDispatchedEvent)
        {
            hasDispatchedEvent = true;
            Debug.Log("GameEndEvent 발생");
            OnGameEnded.Invoke();
        }
        else
        {
            Debug.Log("이벤트가 이미 발생했으므로 무시됨");
        }
    }
}