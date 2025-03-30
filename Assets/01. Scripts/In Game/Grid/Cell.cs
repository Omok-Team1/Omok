using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public void Init((int, int) coordinate, Sprite emptySprite)
    {
        _coordinate = coordinate;
        _cellSprite = GetComponent<SpriteRenderer>();
        _select = GetComponent<Select>();
        
        _cellOwner = Turn.NONE;
        emptyMarker = emptySprite;
        Marker = emptySprite;
        
        _cellSprite.enabled = false;
    }

    public void EraseMarker()
    {
        Debug.Log("Erasing marker!!");
        Marker = emptyMarker;
        _select = gameObject.AddComponent<Select>();
        
        _cellSprite.enabled = false;
    }

    public void SelectedCell(bool isDestroy = false)
    {
        _cellSprite.enabled = true;
        _cellSprite.color = Color.white;

        if (isDestroy is true)
            Destroy(_select);
    }

    public (int, int) _coordinate { get; private set; }

    public int Row => _coordinate.Item1; // Row 추가
    public int Col => _coordinate.Item2; // Col 추가

    private SpriteRenderer _cellSprite;
    private Sprite emptyMarker;
    private Select _select;
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
}
