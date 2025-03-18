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
        _cellOnwer = Turn.NONE;
        Marker = emptySprite;
    }

    public void EraseMarker(Sprite emptySprite)
    {
        Debug.Log("Erasing marker!!");
        Marker = emptySprite;
    }

    public (int, int) _coordinate { get; private set; }

    private SpriteRenderer _cellSprite;
    private Turn _cellOnwer;
    
    public Sprite Marker
    {
        get => _cellSprite.sprite;
        set => _cellSprite.sprite = value;
    }

    public Turn CellOnwer
    {
        get => _cellOnwer;
        set => _cellOnwer = value;
    }
}