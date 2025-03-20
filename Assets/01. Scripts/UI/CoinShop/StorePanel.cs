using UnityEngine;
using DG.Tweening;
using System.Collections;

public class StorePanel : SubUICanvas
{
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private Ease slideEase = Ease.OutBack;

    private RectTransform _rectTransform;
    private Vector2 _originalPosition;
    private bool _isAnimating;
    private Tween _currentTween;
    private Canvas _parentCanvas; // 부모 캔버스 추적

    public bool IsAnimating => _isAnimating; 

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _originalPosition = _rectTransform.anchoredPosition;
        _rectTransform.anchoredPosition = new Vector2(Screen.width, _originalPosition.y);
        
        // ✅ 부모 캔버스 저장 (상위 캔버스가 비활성화되는 걸 방지하기 위해)
        _parentCanvas = GetComponentInParent<Canvas>();
    }

    public override void Show()
    {
        if (_isAnimating || _rectTransform == null) return;

        _currentTween?.Kill();
        _isAnimating = true;

        // 부모 캔버스도 활성화 (상위 캔버스가 비활성화되면 애니메이션이 중간에 멈추는 문제 방지)
        if (_parentCanvas != null)
        {
            _parentCanvas.gameObject.SetActive(true);
        }

        _currentTween = _rectTransform.DOAnchorPosX(_originalPosition.x, slideDuration)
            .SetEase(slideEase)
            .OnComplete(() => _isAnimating = false)
            .OnKill(() => _isAnimating = false);

        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        if (_isAnimating || _rectTransform == null) return;
        if (!gameObject.activeSelf) return;

        _currentTween?.Kill();
        _isAnimating = true;

        _currentTween = _rectTransform.DOAnchorPosX(Screen.width, slideDuration)
            .SetEase(slideEase)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                _isAnimating = false;
                
                // 부모 캔버스도 여기서 비활성화 (애니메이션이 끝난 후 실행)
                if (_parentCanvas != null)
                {
                    _parentCanvas.gameObject.SetActive(false);
                }
            })
            .OnKill(() =>
            {
                gameObject.SetActive(false);
                _isAnimating = false;
            });
    }
    // UIManager가 애니메이션이 끝날 때까지 기다리도록 함
    public void WaitAndCloseCanvas(UIManager uiManager)
    {
        StartCoroutine(WaitForAnimation(uiManager));
    }

    private IEnumerator WaitForAnimation(UIManager uiManager)
    {
        while (_isAnimating)
        {
            yield return null; // 애니메이션이 끝날 때까지 대기
        }

        uiManager.CloseChildrenCanvas(); // 애니메이션 완료 후 다시 실행
    }

    private void OnDestroy()
    {
        _currentTween?.Kill();
    }
}
