using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class RouletteSegment
{
    public float angle;
    public int value;
    public GameObject resultPanel; // 연결된 결과 패널
}

public class RouletteGame : MonoBehaviour
{
    [Header("룰렛 설정")]
    [SerializeField] private Transform rouletteWheel; // 룰렛 스프라이트
    [SerializeField] private Transform centerPoint; // 룰렛의 중앙 기준점 (빈 오브젝트)
    [SerializeField] private float spinSpeed = 300f;
    [SerializeField] private float minSpinTime = 2f;
    [SerializeField] private float maxSpinTime = 5f;
    
    [Header("영역 설정")]
    [SerializeField] private Transform arrowTransform; // 화살표
    [SerializeField] private float startAngle = 0f; // 첫 번째 시작 각도
    [SerializeField] private RouletteSegment[] segments; // 룰렛 세그먼트 배열
    
    [Header("UI 요소")]
    [SerializeField] private Button startButton; // 시작 버튼
    [SerializeField] private TextMeshProUGUI cooldownText; // TextMeshPro 쿨타임 텍스트
    [SerializeField] private Button backButtonPrefab; // 돌아가기 버튼
    
    [Header("이펙트")]
    [SerializeField] private ParticleSystem specialEffect;
    [SerializeField] private int specialEffectThreshold = 50; // 특수 이펙트를 재생할 최소 가치
    
    private float[] segmentStartAngles;
    
    private bool canSpin = true;
    private float cooldownTime = 10f; // 쿨타임 10초
    private bool isSpinning = false;
    
    private CoinManager coinManager; // 코인 매니저 참조
    
    private void Start()
    {
        coinManager = FindObjectOfType<CoinManager>();
        
        // 룰렛 중앙점 검증
        if (centerPoint == null)
        {
            centerPoint = rouletteWheel;
            Debug.LogWarning("룰렛 중앙 기준점이 설정되지 않았습니다. 디폴트 설정으로 룰렛 휠 스프라이트를 기준점으로 사용합니다.");
        }
        
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
        
        // 세그먼트 설정 및 패널 검증
        ValidateSegments();
        InitializeSegmentAngles();
        // 모든 패널 초기화 (비활성화)
        CloseAllPanels();
        // 돌아가기 버튼 설정
        SetupBackButtons();
    }
    
    // 세그먼트 설정 검증
    private void ValidateSegments()
    {
        if (segments == null || segments.Length == 0)
        {
            Debug.LogError("룰렛 세그먼트가 설정되지 않았습니다!");
            // 세그먼트 저장 배열
            segments = new RouletteSegment[6];
            for (int i = 0; i < 6; i++)
            {
                segments[i] = new RouletteSegment { angle = 60f, value = i == 0 ? 0 : (i == 4 ? 100 : i * 10) };
            }
        }
        // 세그먼트 각도 합계 체크
        float totalAngle = 0f;
        for (int i = 0; i < segments.Length; i++)
        {
            totalAngle += segments[i].angle;
            
            // 패널 검증
            if (segments[i].resultPanel == null)
            {
                Debug.LogWarning($"세그먼트 {i} (동전값: {segments[i].value})의 결과 패널이 할당되지 않았습니다!");
            }
        }
        
        // 각도 합계가 360도인지 확인
        if (Mathf.Abs(totalAngle - 360f) > 0.01f)
        {
            Debug.LogWarning($"세그먼트 각도의 합계({totalAngle})가 360도가 아닙니다. 자동으로 조정됩니다.");
            float scaleFactor = 360f / totalAngle;
            for (int i = 0; i < segments.Length; i++)
            {
                segments[i].angle *= scaleFactor;
            }
        }
    }
    
    // 각 세그먼트의 시작 각도 계산
    private void InitializeSegmentAngles()
    {
        // 시작 각도 배열 초기화
        segmentStartAngles = new float[segments.Length];
        // 첫 번째 세그먼트의 시작 각도 설정
        segmentStartAngles[0] = startAngle;
        // 나머지 세그먼트의 시작 각도 계산
        for (int i = 1; i < segments.Length; i++)
        {
            segmentStartAngles[i] = segmentStartAngles[i-1] + segments[i-1].angle;
        // 각도가 360도를 넘어가면 0도로 돌려주기
            segmentStartAngles[i] = NormalizeAngle(segmentStartAngles[i]);
        }
        for (int i = 0; i < segments.Length; i++)
        {
            float endAngle = NormalizeAngle(segmentStartAngles[i] + segments[i].angle);
        }
    }
    
    // 각도를 0~360 범위로 정규화
    private float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle < 0)
        {
            angle += 360f;
        }
        return angle;
    }
    
    // 돌아가기 버튼 설정 - 각 결과 패널에 버튼 찾고 이벤트 연결
    private void SetupBackButtons()
    {
        foreach (var segment in segments)
        {
            if (segment.resultPanel != null)
            {
                Button backButton = segment.resultPanel.GetComponentInChildren<Button>();  
                // 버튼이 없다면 경고 표시
                if (backButton == null)
                {
                    Debug.LogWarning($"{segment.value}의 결과 패널에 버튼이 없습니다. 돌아가기 버튼을 추가하세요.");
                    
                }
                
                // 버튼에 이벤트 연결
                if (backButton != null)
                {
                    backButton.onClick.RemoveAllListeners(); // 기존 이벤트 제거
                    backButton.onClick.AddListener(CloseResultPanel);
                }
            }
        }
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
        float spinTime = Random.Range(minSpinTime, maxSpinTime);
        float elapsedTime = 0f;
        
        while (elapsedTime < spinTime)
        {
            rouletteWheel.Rotate(0, 0, spinSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

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
        
        isSpinning = false;
  
        // 정지한 위치 감지 - 화살표와 룰렛 중심 사이의 각도 계산
        int segmentIndex = GetCurrentSegment();
        OnStop(segmentIndex);
    }
    
    // 현재 화살표가 가리키는 세그먼트 인덱스
    private int GetCurrentSegment()
    {
        if (arrowTransform == null || centerPoint == null)
        {
            Debug.LogError("화살표 또는 룰렛 중앙점 Transform이 할당되지 않았습니다!");
            return 0;
        }

        float wheelRotation = rouletteWheel.eulerAngles.z;
        Vector2 rouletteCenter = centerPoint.position;
        Vector2 arrowPosition = arrowTransform.position;
        Vector2 direction = arrowPosition - rouletteCenter;
        // 각도 계산 (0도는 오른쪽, 시계 방향으로 증가)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // 룰렛 회전 각도를 고려하여 조정
        angle = NormalizeAngle(angle - wheelRotation);
        // 어떤 세그먼트에 속하는지 확인
        for (int i = 0; i < segments.Length; i++)
        {
            float startAngle = segmentStartAngles[i];
            float endAngle = NormalizeAngle(startAngle + segments[i].angle);
            
            // 세그먼트가 0도/360도를 걸쳐 있는 경우
            if (startAngle > endAngle)
            {
                if (angle >= startAngle || angle < endAngle)
                {
                    Debug.Log($"화살표가 세그먼트 {i} (가치: {segments[i].value})을 가리킵니다.");
                    return i;
                }
            }
            // 일반적인 경우
            else if (angle >= startAngle && angle < endAngle)
            {
                Debug.Log($"화살표가 세그먼트 {i} (가치: {segments[i].value})을 가리킵니다.");
                return i;
            }
        }
        
        // 에러
        Debug.LogWarning("어떤 세그먼트도 감지되지 않음, 기본값(0번 세그먼트) 반환");
        return 0;
    }
    
    // 룰렛 정지 후 처리 함수
    private void OnStop(int segmentIndex)
    {
        // 세그먼트 인덱스가 유효한지 검사
        if (segmentIndex < 0 || segmentIndex >= segments.Length)
        {
            Debug.LogWarning($"유효하지 않은 세그먼트 인덱스: {segmentIndex}, 첫 번째 세그먼트로 처리합니다.");
            segmentIndex = 0;
        }
        
        // 해당 세그먼트의 가치 가져오기
        int segmentValue = segments[segmentIndex].value;
        GameObject resultPanel = segments[segmentIndex].resultPanel;
        
        // 결과 패널이 할당되어 있는지 확인
        if (resultPanel == null)
        {
            Debug.LogWarning($"세그먼트 {segmentIndex} (코인값: {segmentValue})의 결과 패널이 할당되지 않았습니다.");
            // 이쪽은 코인 추가만 함
            if (segmentValue > 0)
            {
                Debug.Log($"{segmentValue}코인 획득! 코인이 {segmentValue}개 추가됩니다.");
                if (coinManager != null)
                {
                    coinManager.AddCoin(segmentValue);
                }
            }
            else
            {
                Debug.Log("꽝! 아쉽게도 보상이 없습니다.");
            }
            return;
        }
        
        // 모든 패널 닫고 결과 패널 표시
        CloseAllPanels();
        resultPanel.SetActive(true);
        
        // 코인 추가
        if (segmentValue > 0 && coinManager != null)
        {
            Debug.Log($"{segmentValue}코인 획득! 코인이 {segmentValue}개 추가됩니다.");
            coinManager.AddCoin(segmentValue);
            
            // 특수 이펙트 재생 (당첨될 경우)
            if (segmentValue >= specialEffectThreshold && specialEffect != null)
            {
                specialEffect.Play();
            }
        }
        else
        {
            Debug.Log("꽝! 아쉽게도 보상이 없습니다.");
        }
    }
    
    // 결과 패널 닫기 - 버튼에서 호출
    public void CloseResultPanel()
    {
        CloseAllPanels();
        Debug.Log("결과 패널을 닫았습니다.");
    }
    
    // 모든 패널 닫기
    public void CloseAllPanels()
    {
        // 모든 세그먼트의 결과 패널 비활성화
        foreach (var segment in segments)
        {
            if (segment.resultPanel != null)
            {
                segment.resultPanel.SetActive(false);
            }
        }
    }
    
    // 디버깅용
    private void OnDrawGizmos()
    {
        // 에디터 모드와 플레이 모드 모두에서 표시
        Transform center = centerPoint != null ? centerPoint : (rouletteWheel != null ? rouletteWheel : transform);
        
        // 룰렛 중심 시각화
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(center.position, 30f);
        
        // 화살표 위치 시각화
        if (arrowTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(arrowTransform.position, 20f);
            
            // 룰렛과 화살표 사이 선 그리기
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(center.position, arrowTransform.position);
        }
        
        // 세그먼트 시각화
        if (segments != null && segments.Length > 0)
        {
            float currentAngle = startAngle;
            
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] == null) continue;
                
                Gizmos.color = new Color(0, 1, 0, 0.5f);
                
                float startRad = currentAngle * Mathf.Deg2Rad;
                float endRad = (currentAngle + segments[i].angle) * Mathf.Deg2Rad;
                
                Vector3 startDir = new Vector3(Mathf.Cos(startRad), Mathf.Sin(startRad), 0);
                Vector3 endDir = new Vector3(Mathf.Cos(endRad), Mathf.Sin(endRad), 0);
                
                float radius = 200f; // 시각화용 반지름
                
                Gizmos.DrawLine(center.position, center.position + startDir * radius);
                Gizmos.DrawLine(center.position, center.position + endDir * radius);
                
                // 세그먼트 중앙에 라벨 표시
                float middleRad = (startRad + endRad) / 2;
                Vector3 middleDir = new Vector3(Mathf.Cos(middleRad), Mathf.Sin(middleRad), 0);
                Vector3 labelPos = center.position + middleDir * (radius * 0.7f);
                
                // 세그먼트 값 표시 (Unity 에디터에서만 보임)
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(labelPos, segments[i].value.ToString());
                #endif
                
                // 다음 세그먼트의 시작 각도 업데이트
                currentAngle += segments[i].angle;
                currentAngle = NormalizeAngle(currentAngle);
            }
        }
    }
}