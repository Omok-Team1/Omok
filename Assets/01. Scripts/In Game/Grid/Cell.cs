using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public void Init((int, int) coordinate, Sprite emptySprite)
    {
        _coordinate = coordinate;
        _cellSprite = GetComponent<SpriteRenderer>();
        
        _cellOwner = Turn.NONE;
        Marker = emptySprite;
    }

    public void EraseMarker(Sprite emptySprite)
    {
        Debug.Log("Erasing marker!!");
        Marker = emptySprite;
    }

    public (int, int) _coordinate { get; private set; }

    private SpriteRenderer _cellSprite;
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