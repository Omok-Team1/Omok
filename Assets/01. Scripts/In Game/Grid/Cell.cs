using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    
    public Sprite EmptyMarker => emptyMarker;
    public void Init((int, int) coordinate, Sprite emptySprite)
    {
        _coordinate = coordinate;
        _cellSprite = GetComponent<SpriteRenderer>();
        
        _cellOwner = Turn.NONE;
        emptyMarker = emptySprite;
        Marker = emptySprite;
    }

    public void EraseMarker()
    {
        Debug.Log("Erasing marker!!");
        Marker = emptyMarker;
    }

    public (int, int) _coordinate { get; private set; }

    public int Row => _coordinate.Item1; // Row 추가
    public int Col => _coordinate.Item2; // Col 추가

    private SpriteRenderer _cellSprite;
    private Sprite emptyMarker;
    private Turn _cellOwner;
    
    public Sprite Marker
    {
        get => _cellSprite.sprite;
        set => _cellSprite.sprite = value;
    }

    public Turn CellOwner
    {
        get => _cellOwner;
        set => _cellOwner = value;
    }
    public class CellLogger : MonoBehaviour
    {
        [SerializeField] private Cell _targetCell;
    
        public void SafeEraseMarker()
        {
            _targetCell.Marker = _targetCell.emptyMarker;
        }
    }
}
