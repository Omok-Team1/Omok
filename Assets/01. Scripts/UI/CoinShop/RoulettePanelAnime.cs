using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class RoulettePanelAnime : MonoBehaviour
{
    [Header("Panel Settings")]
    [SerializeField] private RectTransform roulettePanel;
    [SerializeField] private Button showButton;
    [SerializeField] private Button hideButton;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease animationEase = Ease.OutBack;
    
    [Header("Panel Position")]
    [SerializeField] private float hiddenYPosition = -600f;
    [SerializeField] private float visibleYPosition = 0f;

    private bool isPanelVisible = false;

    private void Awake()
    {
        // 초기에 패널 숨기기
        HidePanelInstantly();
    }

    private void Start()
    {
        // 버튼에 이벤트 리스너 추가
        showButton.onClick.AddListener(ShowRoulettePanel);
        if (hideButton != null)
            hideButton.onClick.AddListener(HideRoulettePanel);
            
        // DOTween 재생 중 게임 종료 시 오류 방지
        DOTween.SetTweensCapacity(500, 50);
    }
    
    private void HidePanelInstantly()
    {
        // 패널 위치 초기화 (즉시 숨김)
        Vector2 anchoredPos = roulettePanel.anchoredPosition;
        anchoredPos.y = hiddenYPosition;
        roulettePanel.anchoredPosition = anchoredPos;
        isPanelVisible = false;
    }

    public void ShowRoulettePanel()
    {
        if (isPanelVisible) return;
        
        // 기존 Tween 중지
        DOTween.Kill(roulettePanel);
        
        isPanelVisible = true;
        
        // 시작 스케일 설정
        roulettePanel.localScale = new Vector3(0.95f, 0.95f, 1f);
        
        // 위치 애니메이션
        roulettePanel.DOAnchorPosY(visibleYPosition, animationDuration)
            .SetEase(animationEase)
            .OnComplete(() => {
                // 애니메이션 완료 후 추가 작업이 필요하면 여기에 작성
                Debug.Log("패널 표시 완료");
            });
        
        // 스케일 애니메이션
        roulettePanel.DOScale(1f, animationDuration * 0.8f)
            .SetEase(Ease.OutBack);
        
        // 버튼 숨기기
        showButton.gameObject.SetActive(false);
    }

    public void HideRoulettePanel()
    {
        if (!isPanelVisible) return;
        
        // 기존 Tween 중지
        DOTween.Kill(roulettePanel);
        
        isPanelVisible = false;
        
        // DOTween을 사용하여 패널을 아래로 애니메이션
        roulettePanel.DOAnchorPosY(hiddenYPosition, animationDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => {
                // 애니메이션 완료 후 추가 작업이 필요하면 여기에 작성
                showButton.gameObject.SetActive(true);
                Debug.Log("패널 숨김 완료");
            });
    }
}