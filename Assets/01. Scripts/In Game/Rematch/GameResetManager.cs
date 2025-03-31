using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
public class GameResetManager : MonoBehaviour
{
    public static GameResetManager Instance { get; private set; }

    [SerializeField] private string titleSceneName = "Title";
    [SerializeField] private float delayBeforeReload = 0.1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

 
    public void CleanupDontDestroyObjects()
    {
        // GameEndEventDispatcher, EventManager 등 싱글톤 객체들 초기화
        if (GameEndEventDispatcher.Instance != null)
        {
            GameEndEventDispatcher.Instance.ClearEventListeners();
            GameEndEventDispatcher.Instance.ResetDispatchState();
        }

        // DontDestroyOnLoad로 설정된 다른 매니저 객체들도 초기화
        // 이 스크립트 자신은 제외
        GameObject[] dontDestroyObjects = FindObjectsOfDontDestroyOnLoad();
        foreach (GameObject obj in dontDestroyObjects)
        {
            // 자기 자신은 파괴하지 않음
            if (obj != this.gameObject)
            {
                Debug.Log($"[GameResetManager] DontDestroy 객체 정리: {obj.name}");
                Destroy(obj);
            }
        }
    }

    private GameObject[] FindObjectsOfDontDestroyOnLoad()
    {
        // DontDestroyOnLoad 씬의 루트 오브젝트들 찾기
        GameObject temp = new GameObject();
        DontDestroyOnLoad(temp);
        Scene dontDestroyScene = temp.scene;
        Destroy(temp);

        return dontDestroyScene.GetRootGameObjects();
    }
    
}