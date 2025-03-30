using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ReplayListUIController : MonoBehaviour
{
    [Header("Replay Button Prefab")]
    [SerializeField] private GameObject replayButtonPrefab;

    [Header("UI References")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollWheelSensitivity = 0.1f; // 휠 감도 조절

    
    
    private void Awake()
    {
        
    }

    private void Start()
    {
       
    }

    private void OnEnable()
    {
        
        DontDestroyOnLoad(this.gameObject);
        StartCoroutine(PopulateReplayList());
        Debug.Log("ReplayListUIController OnEnable 호출");
        Debug.Log($"현재 리플레이 수: {ReplayManager.Instance.GetReplays().Count}");
    }
    
    private void OnDisable()
    {
        Debug.Log("ReplayListUIController OnDisable 호출");
    }

    private IEnumerator PopulateReplayList()
    {
        
        foreach (Transform child in contentParent)
        {
            child.gameObject.SetActive(false);
        }
        yield return null;

        // 리플레이 데이터 다시 로드
        ReplayManager.Instance.LoadReplays();
        var replays = ReplayManager.Instance.GetReplays();
    
        // 기존 비활성화된 버튼들 제거
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 리플레이 데이터가 없을 경우 처리
        if (replays.Count == 0)
        {
            Debug.Log("No replays available");
            yield break;
        }

        // 새로운 버튼 생성
        foreach (var replay in replays)
        {
            Instantiate(replayButtonPrefab, contentParent)
                .GetComponent<ReplayButtonController>()?
                .SetReplayData(replay);
        }

        Canvas.ForceUpdateCanvases();
        yield return null;

        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void Update()
    {
        // 마우스 휠 입력 처리
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            scrollRect.velocity = new Vector2(0, scroll * 1000 * scrollWheelSensitivity);
        }
    }
}