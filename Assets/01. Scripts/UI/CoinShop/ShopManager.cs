using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using DG.Tweening; // DOTween 추가

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Button[] payButtons; // 결제 버튼 배열
    [SerializeField] private int[] coinAmounts; // 각 버튼에 해당하는 코인 증가량
    
    [SerializeField] private GameObject payConfirmPanel; // 결제 확인 패널
    [SerializeField] private GameObject paySuccessPanel; // 결제 성공 패널
    [SerializeField] private GameObject payFailPanel; // 결제 실패 패널
    [SerializeField] private TextMeshProUGUI paySuccessT; // 결제 성공 시 표시되는 텍스트 UI
    
    [SerializeField] private Button payConfirmYesButton; // 결제 확인 패널의 '예' 버튼
    [SerializeField] private Button payConfirmNoButton; // 결제 확인 패널의 '아니오' 버튼
    [SerializeField] private Button paySuccessConfirmButton; // 결제 성공 패널의 '확인' 버튼
    [SerializeField] private Button payFailConfirmButton; // 결제 실패 패널의 '확인' 버튼

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.4f; // 애니메이션 시간
    [SerializeField] private Ease openEase = Ease.OutBack; // 패널 열리는 효과
    [SerializeField] private Ease closeEase = Ease.InBack; // 패널 닫히는 효과
    
    private int currentCoinAmount; // 결제 패널에서 현재 선택된 코인 증가량
    
    // 패널의 초기 크기 저장하는 변수들
    private Vector3 confirmPanelOriginalScale;
    private Vector3 successPanelOriginalScale;
    private Vector3 failPanelOriginalScale;
    
    private void Awake()
    {
		// 패널 중복실행 버그 방지
        SaveOriginalScales();
        RemoveAllButtonListeners();
        InitializePanels();
    }
    
    // 패널 크기 버그 방지
    private void SaveOriginalScales()
    {
        if (payConfirmPanel != null)
            confirmPanelOriginalScale = payConfirmPanel.transform.localScale;
            
        if (paySuccessPanel != null)
            successPanelOriginalScale = paySuccessPanel.transform.localScale;
            
        if (payFailPanel != null)
            failPanelOriginalScale = payFailPanel.transform.localScale;
    }
    
    private void Start()
    {
        // 버튼 버그 방지
        SetupButtonListeners();
        
        // DOTween 초기화
        DOTween.SetTweensCapacity(500, 50);
        
        
    }
    
    // 패널 버그 방지 2
    private void InitializePanels()
    {
        // 모든 패널을 비활성화하고 크기 어긋나지 않게 조정
        if (payConfirmPanel != null)
        {
            payConfirmPanel.SetActive(false);
            payConfirmPanel.transform.localScale = Vector3.zero;
        }
        
        if (paySuccessPanel != null)
        {
            paySuccessPanel.SetActive(false);
            paySuccessPanel.transform.localScale = Vector3.zero;
        }
        
        if (payFailPanel != null)
        {
            payFailPanel.SetActive(false);
            payFailPanel.transform.localScale = Vector3.zero;
        }
    }
    
    // 버튼 버그 방지
    private void RemoveAllButtonListeners()
    {
        // 버튼이 없는데 눌리는 현상이나 실행중이 아닌 데 버튼 클릭되는 거 막기
        foreach (Button button in payButtons)
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
            }
        }
        if (payConfirmYesButton != null) payConfirmYesButton.onClick.RemoveAllListeners();
        if (payConfirmNoButton != null) payConfirmNoButton.onClick.RemoveAllListeners();
        if (paySuccessConfirmButton != null) paySuccessConfirmButton.onClick.RemoveAllListeners();
        if (payFailConfirmButton != null) payFailConfirmButton.onClick.RemoveAllListeners();
    }
    
    // 버튼 인식 설정
    private void SetupButtonListeners()
    {
        // 결제 버튼 리스너
        for (int i = 0; i < payButtons.Length; i++)
        {
            int index = i; // 버튼 별 데이터 입력을 배열로 구현
            if (payButtons[i] != null)
            {
                payButtons[i].onClick.AddListener(() => OnPayButtonClick(index));
            }
        }
        
        // 결제 확인 패널 버튼 리스너
        if (payConfirmYesButton != null)
        {
            payConfirmYesButton.onClick.AddListener(OnPayConfirm);
        }
        
        if (payConfirmNoButton != null)
        {
            payConfirmNoButton.onClick.AddListener(OnPayCancel);
        }
        
        // 결제 성공 패널 확인 버튼 리스너
        if (paySuccessConfirmButton != null)
        {
            paySuccessConfirmButton.onClick.AddListener(CloseSuccessPanel);
        }
        
        // 결제 실패 패널 확인 버튼 리스너
        if (payFailConfirmButton != null)
        {
            payFailConfirmButton.onClick.AddListener(CloseFailPanel);
        }
    }
    
    // 결제 버튼 클릭 시 호출되는 함수
    public void OnPayButtonClick(int buttonIndex)
    {
        if (buttonIndex < coinAmounts.Length)
        {
            currentCoinAmount = coinAmounts[buttonIndex];
            
            // 모든 패널을 닫고 결제 확인 패널만 활성화
            CloseAllPanelsImmediately();
            ShowPanelWithAnimation(payConfirmPanel);
            
            Debug.Log($"결제 확인 패널이 열렸습니다. 선택된 코인 증가량: {currentCoinAmount}");
        }
    }
    
    // 결제 확인 패널에서 확인 버튼 클릭 시 호출되는 함수
    public void OnPayConfirm()
    {
        // 결제 확인 패널을 애니메이션으로 닫기
        ClosePanelWithAnimation(payConfirmPanel, () => {
            // 결제 처리 (90% 확률로 성공)
            bool purchaseSuccessful = Random.value < 0.9f;
            
            if (purchaseSuccessful)
            {
                // 성공 텍스트 업데이트
                if (paySuccessT != null)
                {
                    paySuccessT.text = $"+{currentCoinAmount}";
                }
                
                // 코인 추가
                if (CoinManager.Instance != null)
                {
                    CoinManager.Instance.AddCoin(currentCoinAmount);
                }
                
                // 성공 패널 활성화 (애니메이션)
                ShowPanelWithAnimation(paySuccessPanel);
                Debug.Log($"결제 성공! {currentCoinAmount}코인이 추가되었습니다.");
            }
            else
            {
                // 실패 패널 활성화 (애니메이션)
                ShowPanelWithAnimation(payFailPanel);
                Debug.Log("결제에 실패했습니다. 코인이 추가되지 않았습니다.");
            }
        });
    }
    
    // 결제 확인 패널에서 취소 버튼 클릭 시 호출되는 함수
    public void OnPayCancel()
    {
        ClosePanelWithAnimation(payConfirmPanel);
        Debug.Log("결제가 취소되었습니다.");
    }
    
    // 결제 성공 패널 닫기
    public void CloseSuccessPanel()
    {
        ClosePanelWithAnimation(paySuccessPanel);
        Debug.Log("결제 성공 패널이 닫혔습니다.");
    }
    
    // 결제 실패 패널 닫기
    public void CloseFailPanel()
    {
        ClosePanelWithAnimation(payFailPanel);
        Debug.Log("결제 실패 패널이 닫혔습니다.");
    }
    
    // 패널을 애니메이션으로 보여주는 함수
    private void ShowPanelWithAnimation(GameObject panel)
    {
        if (panel != null)
        {
            // 먼저 패널을 활성화하고 초기 스케일 설정
            panel.SetActive(true);
            panel.transform.localScale = Vector3.zero;
            
            // 패널에 따라 초기에 만든 스케일로 애니메이션 적용
            Vector3 targetScale;
            if (panel == payConfirmPanel)
                targetScale = confirmPanelOriginalScale;
            else if (panel == paySuccessPanel)
                targetScale = successPanelOriginalScale;
            else if (panel == payFailPanel)
                targetScale = failPanelOriginalScale;
            else
                targetScale = Vector3.one;
            panel.transform.DOScale(targetScale, animationDuration)
                .SetEase(openEase)
                .OnComplete(() => {
                    // 애니메이션 완료 후 디버그 로그 재생
                    Debug.Log($"{panel.name} 패널이 열렸습니다.");
                });
        }
    }
    
    // 패널을 애니메이션으로 닫는 함수
    private void ClosePanelWithAnimation(GameObject panel, TweenCallback onComplete = null)
    {
        if (panel != null && panel.activeInHierarchy)
        {
            panel.transform.DOScale(Vector3.zero, animationDuration)
                .SetEase(closeEase)
                .OnComplete(() => {
                    panel.SetActive(false);
                    // 추가 콜백이 있으면 실행
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                    Debug.Log($"{panel.name} 패널이 닫혔습니다.");
                });
        }
        else if (onComplete != null)
        {
            // 패널이 이미 비활성화 상태라면 바로 콜백 실행
            onComplete();
        }
    }
    
    // 모든 패널을 즉시 닫는 함수 (애니메이션 없이)
    private void CloseAllPanelsImmediately()
    {
        // 모든 진행 중인 DOTween 애니메이션 중지
        DOTween.Kill(payConfirmPanel.transform);
        DOTween.Kill(paySuccessPanel.transform);
        DOTween.Kill(payFailPanel.transform);
        
        // 모든 패널 비활성화
        payConfirmPanel.SetActive(false);
        paySuccessPanel.SetActive(false);
        payFailPanel.SetActive(false);
    }
    
    // 외부에서 모든 패널을 닫을 수 있는 공용 메서드 (애니메이션 적용)
    public void CloseAllPanels()
    {
        // 활성화된 패널만 애니메이션으로 닫기
        if (payConfirmPanel.activeInHierarchy)
            ClosePanelWithAnimation(payConfirmPanel);
            
        if (paySuccessPanel.activeInHierarchy)
            ClosePanelWithAnimation(paySuccessPanel);
            
        if (payFailPanel.activeInHierarchy)
            ClosePanelWithAnimation(payFailPanel);
            
    }
    
    
    // 패널 버그 방지 3
    private void OnEnable()
    {
        CloseAllPanelsImmediately();
    }
    
    private void OnDestroy()
    {
        // 씬 전환 시 DOTween 메모리 누수 방지
        DOTween.Kill(payConfirmPanel.transform);
        DOTween.Kill(paySuccessPanel.transform);
        DOTween.Kill(payFailPanel.transform);
    }
}