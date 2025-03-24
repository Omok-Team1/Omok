using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class StorePanel : SubUICanvas
{
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private Ease slideEase = Ease.OutBack;
    [SerializeField] private Canvas targetParentCanvas;
    
    private List<GameObject> _alwaysActiveObjects = new List<GameObject>();
    private RectTransform _rectTransform;
    private Vector2 _originalPosition;
    private bool _isAnimating;
    private Tween _currentTween;
    private Canvas _parentCanvas; // 부모 캔버스 추적

    public bool IsAnimating => _isAnimating;

    // 항상 활성화 상태를 유지할 오브젝트 등록 메서드
    public void RegisterAlwaysActiveObject(GameObject obj)
    {
        if (obj != null && !_alwaysActiveObjects.Contains(obj))
        {
            _alwaysActiveObjects.Add(obj);
        }
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _originalPosition = _rectTransform.anchoredPosition;
        _rectTransform.anchoredPosition = new Vector2(Screen.width, _originalPosition.y);

        // 부모 캔버스 저장
        _parentCanvas = GetComponentInParent<Canvas>();
    }

    public override void Show()
    {
        // 부모 캔버스 활성화
        if (targetParentCanvas != null && !targetParentCanvas.gameObject.activeSelf)
        {
            targetParentCanvas.gameObject.SetActive(true);
        }
        else if (_parentCanvas != null && !_parentCanvas.gameObject.activeSelf)
        {
            _parentCanvas.gameObject.SetActive(true);
        }

        // 항상 활성화 상태를 유지해야 할 오브젝트들 활성화
        foreach (var obj in _alwaysActiveObjects)
        {
            if (obj != null && !obj.activeSelf)
            {
                obj.SetActive(true);
            }
        }

        _currentTween?.Kill();
        _isAnimating = true;

        _currentTween = _rectTransform.DOAnchorPosX(_originalPosition.x, slideDuration)
            .SetEase(slideEase)
            .OnComplete(() => _isAnimating = false);

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
                    // 항상 활성화되어야 할 오브젝트들은 먼저 활성화
                    foreach (var obj in _alwaysActiveObjects)
                    {
                        if (obj != null)
                        {
                            obj.SetActive(true);
                        }
                    }

                    _parentCanvas.gameObject.SetActive(false);
                }
            })
            .OnKill(() =>
            {
                gameObject.SetActive(false);
                _isAnimating = false;
            });
    }

    // 애니메이션 완료 후 UIManager.CloseChildrenCanvas 호출
    public void HideAndCloseUI()
    {
        if (_isAnimating || _rectTransform == null) return;
        if (!gameObject.activeSelf) return;

        _currentTween?.Kill();
        _isAnimating = true;

        _currentTween = _rectTransform.DOAnchorPosX(Screen.width, slideDuration)
            .SetEase(slideEase)
            .OnComplete(() =>
            {
                _isAnimating = false;
                
                // 항상 활성화된 상태로 유지할 오브젝트들 처리
                foreach (var obj in _alwaysActiveObjects)
                {
                    if (obj != null)
                    {
                        obj.SetActive(true);
                    }
                }
                
                // 애니메이션이 완료된 후 UIManager 호출
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.CloseChildrenCanvas();
                }
            })
            .OnKill(() => { _isAnimating = false; });
    }

    // UIManager가 애니메이션이 끝날 때까지 기다리도록 함
    public IEnumerator WaitForAnimation(UIManager uiManager)
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