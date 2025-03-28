using UnityEngine;
using System.Collections.Generic;

public class ReplayStoneManager : MonoBehaviour
{
    [Header("돌 설정")]
    [SerializeField] private Sprite blackStoneSprite;  // 흑돌 스프라이트
    [SerializeField] private Sprite whiteStoneSprite;  // 백돌 스프라이트

    [Header("보드 설정")]
    [SerializeField] private Transform boardCenterPoint;  // 보드의 중심점 설정
    [SerializeField] private float cellSize = 1.0f;  // 격자 간 간격

    [Header("참조")]
    [SerializeField] private BoardGrid boardGrid;  // 보드 그리드 참조

    // 생성된 돌들을 추적하기 위한 리스트
    private List<GameObject> placedStones = new List<GameObject>();

    /// <summary>
    /// 논리적 좌표를 실제 화면 좌표로 변환하는 메서드
    /// </summary>
    private Vector3 ConvertLogicalToScreenCoordinate(int row, int col)
    {
        // 15x15 보드에서의 실제 위치 계산 (중심점 (7,7) 기준)
        float xOffset = col * cellSize; // -7~7 범위 직접 사용
        float yOffset = row * cellSize;
        return boardCenterPoint.position + new Vector3(xOffset, yOffset, 0);
    }

    /// <summary>
    /// 특정 위치에 돌을 배치하는 메서드
    /// </summary>
    public void PlaceStone(int row, int col, int cellOwner)
    {
        // 해당 좌표의 셀 확인
        Cell targetCell = boardGrid.GetCell(row, col);
        if (targetCell == null)
        {
            Debug.LogWarning($"Invalid cell coordinates: row {row}, col {col}");
            return;
        }

        // 화면 좌표 계산
        Vector3 screenPosition = ConvertLogicalToScreenCoordinate(row, col);

        // 돌 스프라이트 선택
        Sprite stoneSprite = (cellOwner == (int)Turn.PLAYER1) ? blackStoneSprite : whiteStoneSprite;

        // 돌 생성
        GameObject stonePrefab = new GameObject($"Stone_{row}_{col}");
        SpriteRenderer spriteRenderer = stonePrefab.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = stoneSprite;
        spriteRenderer.sortingLayerName = "Stones"; // 새 레이어 생성
        spriteRenderer.sortingOrder = 5;
        
        // 배치 및 트랜스폼 설정
        stonePrefab.transform.position = screenPosition;
        stonePrefab.transform.SetParent(transform);  // ReplayStoneManager의 자식으로 설정

        // 생성된 돌 추적
        placedStones.Add(stonePrefab);

        // 셀에 마커 설정
        targetCell.Marker = stoneSprite;
        targetCell.CellOwner = (Turn)cellOwner;
    }

    /// <summary>
    /// 모든 돌을 제거하는 메서드
    /// </summary>
    public void ClearAllStones()
    {
        // 모든 생성된 돌 제거
        foreach (GameObject stone in placedStones)
        {
            Destroy(stone);
        }
        placedStones.Clear();

        // 보드 그리드의 모든 셀 초기화
        boardGrid.Clear();
    }
}