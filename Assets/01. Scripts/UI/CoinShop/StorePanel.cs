using UnityEngine;
using DG.Tweening;

public class StorePanel : SubUICanvas
{
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private Ease slideEase = Ease.OutBack;

    private RectTransform _rectTransform;
    private Vector2 _originalPosition;
    private bool _isAnimating;
    private Tween _currentTween; // 현재 실행 중인 Tween 추적

    private void Awake()
    {
        // Awake에서 RectTransform 초기화 (활성화 여부와 무관하게 실행)
        _rectTransform = GetComponent<RectTransform>();
        _originalPosition = _rectTransform.anchoredPosition;
    }

    public override void Init()
    {
        base.Init();
        
        // 초기 위치 설정
        if (_rectTransform != null)
        {
            _rectTransform.anchoredPosition = new Vector2(Screen.width, _originalPosition.y);
        }
    }

    public override void Show()
    {
        Debug.Log("StorePanel Show 호출됨");
        
        // 애니메이션 중이거나 RectTransform이 없으면 리턴
        if (_isAnimating || _rectTransform == null) return;

        // 기존 Tween 정리
        _currentTween?.Kill();
        _isAnimating = true;

        // 시작 위치 설정
        _rectTransform.anchoredPosition = new Vector2(Screen.width, _originalPosition.y);

        // 슬라이드 인 애니메이션
        _currentTween = _rectTransform.DOAnchorPosX(_originalPosition.x, slideDuration)
            .SetEase(slideEase)
            .OnComplete(() => _isAnimating = false)
            .OnKill(() => _isAnimating = false);
    }

    public override void Hide()
    {
        Debug.Log("StorePanel Hide 호출됨");
        
        // 애니메이션 중이거나 RectTransform이 없으면 리턴
        if (_isAnimating || _rectTransform == null) return;
        
        // 비활성화 상태라면 아무것도 하지 않음
        if (!gameObject.activeSelf) return;
        
        // 기존 Tween 정리
        _currentTween?.Kill();
        _isAnimating = true;

        // 슬라이드 아웃 애니메이션
        _currentTween = _rectTransform.DOAnchorPosX(Screen.width, slideDuration)
            .SetEase(slideEase)
            .OnComplete(() => {
                gameObject.SetActive(false);
                _isAnimating = false;
            })
            .OnKill(() => {
                gameObject.SetActive(false);
                _isAnimating = false;
            });
    }

    private void OnDestroy()
    {
        // 객체 파괴 시 Tween 정리
        _currentTween?.Kill();
    }
}