using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class RouletteGame : MonoBehaviour
{
    [Header("룰렛 설정")]
    [SerializeField] private Transform rouletteWheel; // 회전할 룰렛 스프라이트
    [SerializeField] private float spinSpeed = 300f; // 회전 속도
    [SerializeField] private float minSpinTime = 2f; // 최소 회전 시간
    [SerializeField] private float maxSpinTime = 5f; // 최대 회전 시간
    
    [Header("영역 인식 설정")]
    [SerializeField] private CircleCollider2D arrowCollider; // 화살표 콜라이더
    [SerializeField] private Image[] segmentImages; // 각 영역의 투명 이미지 배열 (유지)
    
    [Header("UI 요소")]
    [SerializeField] private Button startButton; // 시작 버튼
    [SerializeField] private TextMeshProUGUI cooldownText; // TextMeshPro 쿨타임 텍스트
    
    [Header("결과 패널")]
    [SerializeField] private GameObject panel0; // 꽝 패널
    [SerializeField] private GameObject panel1; // 1코인 패널
    [SerializeField] private GameObject panel10; // 10코인 패널
    [SerializeField] private GameObject panel50; // 50코인 패널
    [SerializeField] private GameObject panel100; // 100코인 패널
    
    [Header("돌아가기 버튼")]
    [SerializeField] private Button backButton0; // 꽝 패널의 돌아가기 버튼
    [SerializeField] private Button backButton1; // 1코인 패널의 돌아가기 버튼
    [SerializeField] private Button backButton10; // 10코인 패널의 돌아가기 버튼
    [SerializeField] private Button backButton50; // 50코인 패널의 돌아가기 버튼
    [SerializeField] private Button backButton100; // 100코인 패널의 돌아가기 버튼
    
    [Header("이펙트")]
    [SerializeField] private ParticleSystem specialEffect; // 100코인 특수 이펙트
    
    private bool canSpin = true; // 룰렛 회전 가능 여부
    private float cooldownTime = 10f; // 쿨타임 10초
    private bool isSpinning = false; // 현재 회전 중인지 여부
    
    private CoinManager coinManager; // 코인 매니저 참조
    
    private void Start()
    {
        coinManager = FindObjectOfType<CoinManager>();
        
        // 시작 버튼 체크
        if (startButton != null)
        {
            startButton.onClick.AddListener(SpinRoulette);
        }
        else
        {
            Debug.LogError("시작 버튼이 할당되지 않았습니다!");
        }
        
        // 초기 쿨타임 텍스트 설정
        if (cooldownText != null)
        {
            cooldownText.text = "룰렛 준비 완료!";
        }
        
        // 돌아가기 버튼 리스너 추가
        SetupBackButtons();
        
        // 모든 패널 초기화 (비활성화)
        CloseAllPanels();
        
        // 화살표 콜라이더 및 세그먼트 이미지 검증
        ValidateComponents();
    }
    
    // 화살표 콜라이더 및 세그먼트 이미지 검증
    private void ValidateComponents()
    {
        // 화살표 콜라이더 검증
        if (arrowCollider == null)
        {
            Debug.LogWarning("화살표 콜라이더가 할당되지 않았습니다!");
        }
        else
        {
            Debug.Log("화살표 콜라이더가 정상적으로 할당되었습니다: " + arrowCollider.name);
        }
        
        // 세그먼트 이미지 검증
        if (segmentImages == null || segmentImages.Length == 0)
        {
            Debug.LogWarning("룰렛 영역용 이미지가 할당되지 않았습니다!");
        }
        else 
        {
            Debug.Log("총 " + segmentImages.Length + "개의 룰렛 영역용 이미지가 등록되었습니다.");
            
            // 각 이미지 확인
            for (int i = 0; i < segmentImages.Length; i++)
            {
                if (segmentImages[i] != null)
                {
                    // 투명한 이미지인지 확인 (디버깅 용도)
                    Color color = segmentImages[i].color;
                    Debug.Log("세그먼트 " + i + " 이미지: " + segmentImages[i].name + 
                             " (투명도: " + color.a + ")");
                }
                else
                {
                    Debug.LogWarning("세그먼트 " + i + " 이미지가 null입니다!");
                }
            }
        }
    }
    
    // 돌아가기 버튼 설정
    private void SetupBackButtons()
    {
        if (backButton0 != null) backButton0.onClick.AddListener(CloseAllPanels);
        if (backButton1 != null) backButton1.onClick.AddListener(CloseAllPanels);
        if (backButton10 != null) backButton10.onClick.AddListener(CloseAllPanels);
        if (backButton50 != null) backButton50.onClick.AddListener(CloseAllPanels);
        if (backButton100 != null) backButton100.onClick.AddListener(CloseAllPanels);
    }
    
    // 룰렛 회전 시작 함수
    public void SpinRoulette()
    {
        if (canSpin && !isSpinning)
        {
            // 패널이 열려 있으면 닫기
            CloseAllPanels();
            
            isSpinning = true;
            StartCoroutine(RandomStop());
            StartCoroutine(Cooltime());
        }
        else if (!canSpin)
        {
            Debug.Log("룰렛은 10초의 쿨타임이 있습니다.");
        }
    }
    
    // 쿨타임 함수 - 10초 동안 룰렛 회전 불가
    private IEnumerator Cooltime()
    {
        canSpin = false;
        startButton.interactable = false;
        
        float remainingTime = cooldownTime;
        
        while (remainingTime > 0)
        {
            cooldownText.text = "쿨타임: " + Mathf.Ceil(remainingTime).ToString() + "초";
            yield return null;
            remainingTime -= Time.deltaTime;
        }
        
        canSpin = true;
        startButton.interactable = true;
        cooldownText.text = "룰렛 준비 완료!";
    }
    
    // 랜덤 정지 함수 - 룰렛을 회전시키고 랜덤 시간 후 정지
    private IEnumerator RandomStop()
    {
        // 랜덤 시간 동안 회전
        float spinTime = Random.Range(minSpinTime, maxSpinTime);
        float elapsedTime = 0f;
        
        while (elapsedTime < spinTime)
        {
            rouletteWheel.Rotate(0, 0, spinSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 점점 속도 줄이기
        float slowDownTime = 2f;
        float currentSpeed = spinSpeed;
        elapsedTime = 0f;
        
        while (elapsedTime < slowDownTime)
        {
            currentSpeed = Mathf.Lerp(spinSpeed, 0, elapsedTime / slowDownTime);
            rouletteWheel.Rotate(0, 0, currentSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 완전히 정지
        isSpinning = false;
        
        // 정지한 위치 감지 - 화살표가 위치한 영역 확인
        int segmentIndex = CheckArrowOverlap();
        OnStop(segmentIndex);
    }
    
    // 화살표 콜라이더 위치에서 영역 이미지 확인
    private int CheckArrowOverlap()
    {
        if (arrowCollider == null)
        {
            Debug.LogError("화살표 콜라이더가 없습니다!");
            return 0; // 기본값 반환
        }
        
        Debug.Log("화살표 콜라이더 위치: " + arrowCollider.transform.position);
        
        // 화살표 콜라이더의 위치
        Vector2 arrowPosition = arrowCollider.transform.position;
        
        // 각 세그먼트 이미지를 확인하여 화살표 콜라이더가 위에 있는지 확인
        for (int i = 0; i < segmentImages.Length; i++)
        {
            Image image = segmentImages[i];
            if (image == null) continue;
            
            // UI 요소의 RectTransform 가져오기
            RectTransform rectTransform = image.rectTransform;
            
            // UI 이미지가 화살표 콜라이더 위치를 포함하는지 확인
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, 
                Camera.main.WorldToScreenPoint(arrowPosition), Camera.main))
            {
                Debug.Log("화살표 콜라이더가 세그먼트 " + i + "에 위치함");
                return i;
            }
        }
        
        // 어떤 영역도 감지되지 않은 경우 추가 확인 방법 시도
        int closestSegment = FindClosestSegment(arrowPosition);
        Debug.Log("영역 감지 실패, 가장 가까운 세그먼트 " + closestSegment + " 선택");
        return closestSegment;
    }
    
    // 화살표와 가장 가까운 세그먼트 찾기
    private int FindClosestSegment(Vector2 arrowPosition)
    {
        float closestDistance = float.MaxValue;
        int closestSegment = 0;
        
        for (int i = 0; i < segmentImages.Length; i++)
        {
            if (segmentImages[i] == null) continue;
            
            // 이미지의 RectTransform 가져오기
            RectTransform rectTransform = segmentImages[i].rectTransform;
            
            // 이미지 중심점 찾기
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            Vector2 imageCenter = (corners[0] + corners[2]) / 2;
            
            // 화살표와의 거리 계산
            float distance = Vector2.Distance(arrowPosition, imageCenter);
            
            // 더 가까운 세그먼트 발견 시 업데이트
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSegment = i;
            }
        }
        
        return closestSegment;
    }
    
    // 룰렛 정지 후 처리 함수
    private void OnStop(int segmentNumber)
    {
        // 영역에 따른 패널 표시 및 코인 조정
        switch (segmentNumber)
        {
            case 0: // 꽝
                RulletPanel0PopUp();
                break;
            case 1: // 1코인
                RulletPanel1PopUp();
                break;
            case 2: // 10코인
                RulletPanel10PopUp();
                break;
            case 3: // 50코인
                RulletPanel50PopUp();
                break;
            case 4: // 100코인
                RulletPanel100PopUp();
                break;
            case 5: // 1
                RulletPanel1PopUp();
                break;
            default:
                // 다른 값이 들어온 경우도 처리
                if (segmentNumber > 5)
                {
                    // 범위를 벗어난 높은 값은 최고 보상으로 처리
                    RulletPanel100PopUp();
                }
                else
                {
                    // 범위를 벗어난 낮은 값은 꽝으로 처리
                    RulletPanel0PopUp();
                }
                break;
        }
    }
    
    // 모든 패널 닫기 (public으로 변경하여 버튼에서 직접 호출 가능)
    public void CloseAllPanels()
    {
        panel0.SetActive(false);
        panel1.SetActive(false);
        panel10.SetActive(false);
        panel50.SetActive(false);
        panel100.SetActive(false);
    }
    
    // 꽝 패널 표시 함수
    private void RulletPanel0PopUp()
    {
        CloseAllPanels();
        panel0.SetActive(true);
        Debug.Log("꽝! 아쉽게도 보상이 없습니다.");
    }
    
   // 1코인 패널 표시 함수
    private void RulletPanel1PopUp()
    {
        CloseAllPanels();
        panel1.SetActive(true);
        Debug.Log("1코인 획득! 코인이 1개 추가됩니다.");
        coinManager.AddCoin(1);
    }
    
    // 10코인 패널 표시 함수
    private void RulletPanel10PopUp()
    {
        CloseAllPanels();
        panel10.SetActive(true);
        Debug.Log("10코인 획득! 코인이 10개 추가됩니다.");
        coinManager.AddCoin(10);
    }
    
    // 50코인 패널 표시 함수
    private void RulletPanel50PopUp()
    {
        CloseAllPanels();
        panel50.SetActive(true);
        Debug.Log("50코인 획득!!! 코인이 50개 추가됩니다.");
        coinManager.AddCoin(50);
    }
    
    // 100코인 패널 표시 함수
    private void RulletPanel100PopUp()
    {
        CloseAllPanels();
        panel100.SetActive(true);
        Debug.Log("100코인 획득!!! 축하합니다! 코인이 100개 추가됩니다.");
        coinManager.AddCoin(100);
        
        // 특수 이펙트 재생
        if (specialEffect != null)
        {
            specialEffect.Play();
        }
    }
}