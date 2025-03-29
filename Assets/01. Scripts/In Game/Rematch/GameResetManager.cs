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

    // 리매치 버튼에 연결할 메서드
    public void CompleteGameReset()
    {
        StartCoroutine(ResetGameCoroutine());
    }

    private IEnumerator ResetGameCoroutine()
    {
        Debug.Log("[GameResetManager] 게임 완전 초기화 시작...");

        // 1. 모든 DontDestroyOnLoad 객체 정리
        CleanupDontDestroyObjects();

        // 2. 먼저 타이틀 씬으로 돌아감
        AsyncOperation titleLoadOperation = SceneManager.LoadSceneAsync(titleSceneName);
        while (!titleLoadOperation.isDone)
        {
            yield return null;
        }

        Debug.Log("[GameResetManager] 타이틀 씬 로드 완료");

        // 약간의 지연을 주어 씬이 완전히 로드되고 초기화되도록 함
        yield return new WaitForSeconds(delayBeforeReload);

        // 3. 타이틀 씬에서 시작 버튼을 프로그래밍 방식으로 클릭
        SimulateStartButtonClick();

        Debug.Log("[GameResetManager] 게임 재시작 완료");
    }

    private void CleanupDontDestroyObjects()
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

    private void SimulateStartButtonClick()
    {
        // 타이틀 씬에서 시작 버튼을 찾아 클릭
        // 버튼의 이름이나 구조에 따라 이 부분을 적절히 수정해야 함
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (Button button in allButtons)
        {
            // 버튼 이름이나 태그로 시작 버튼 식별
            if (button.CompareTag("StartButton"))
            {
                Debug.Log("[GameResetManager] 시작 버튼을 자동으로 클릭합니다.");
                button.onClick.Invoke();
                return;
            }
        }

        Debug.LogWarning("[GameResetManager] 시작 버튼을 찾을 수 없습니다!");
    }
}