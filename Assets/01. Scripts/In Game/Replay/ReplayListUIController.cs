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

    private void Start()
    {
        StartCoroutine(PopulateReplayList());
    }

    private IEnumerator PopulateReplayList()
    {
        // 기존 자식 제거
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        yield return null;

        // 리플레이 데이터 로드
        var replays = ReplayManager.Instance.GetReplays();
        
        // 버튼 생성
        foreach (var replay in replays)
        {
            Instantiate(replayButtonPrefab, contentParent)
                .GetComponent<ReplayButtonController>()?
                .SetReplayData(replay);
        }

        // 레이아웃 강제 갱신
        Canvas.ForceUpdateCanvases();
        yield return null;

        // 초기 위치를 상단으로 설정 (최신 항목 노출)
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