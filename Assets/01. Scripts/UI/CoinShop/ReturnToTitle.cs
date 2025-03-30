using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class ReturnToTitle : MonoBehaviour
{
    [SerializeField] private SceneId titleSceneId; // 타이틀 씬 ID 참조
    [SerializeField] private string titleSceneName = ""; // 또는 씬 이름으로도 설정 가능

    private Button returnButton;

    private void Awake()
    {
        returnButton = GetComponent<Button>();
        
        if (returnButton != null)
        {
            // 기존 리스너 모두 제거
            returnButton.onClick.RemoveAllListeners();
            // 새 리스너 추가
            returnButton.onClick.AddListener(ReturnToTitleScene);
        }
        else
        {
            Debug.LogError("ReturnToTitle: Button 컴포넌트를 찾을 수 없습니다!");
        }
    }

    private void OnEnable()
    {
        // 객체가 활성화될 때마다 이벤트 리스너 재확인
        if (returnButton != null && returnButton.onClick.GetPersistentEventCount() == 0)
        {
            returnButton.onClick.AddListener(ReturnToTitleScene);
        }
    }

    private void ReturnToTitleScene()
    {
        Debug.Log("ReturnToTitle: 타이틀 씬으로 돌아가기 시도");
        
        // 씬 전환 전 정리 작업
        CleanupBeforeSceneChange();
        
        // SceneId를 사용하여 씬 전환 (SwitchSceneButton과 동일한 방식)
        if (titleSceneId != null)
        {
            titleSceneId.Load();
        }
        // 백업 방법: 씬 이름으로 로드
        else if (!string.IsNullOrEmpty(titleSceneName))
        {
            SceneManager.LoadScene(titleSceneName);
        }
        else
        {
            Debug.LogError("ReturnToTitle: 타이틀 씬 참조가 설정되지 않았습니다!");
        }
    }

    private void CleanupBeforeSceneChange()
    {
        // 이 메서드에서는 씬 전환 전에 필요한 정리 작업을 수행합니다
        
        // 1. 정적 참조나 싱글톤 리셋 필요시
        ResetStaticReferences();
        
        // 2. 이벤트 리스너 해제 필요시
        UnregisterEventListeners();
        
        // 3. 씬 전환 후 초기화를 위한 PlayerPrefs 설정
        PlayerPrefs.SetInt("ReturnedFromGame", 1);
        PlayerPrefs.Save();
        
        Debug.Log("ReturnToTitle: 씬 전환 전 정리 작업 완료");
    }
    
    private void ResetStaticReferences()
    {
        // 필요한 경우 정적 참조나 싱글톤을 초기화하는 코드
        // 예: SomeManager.ResetInstance(); 
    }
    
    private void UnregisterEventListeners()
    {
        // 전역 이벤트 리스너가 있다면 해제
        // 예: EventSystem.Instance.RemoveListener(this);
    }
}