using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[
    RequireComponent(typeof(SpriteRenderer))
]
public class Select : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void OnMouseEnter()
    {
        spriteRenderer.enabled = true;
        spriteRenderer.color = Color.green;
    }

    void OnMouseExit()
    {
        spriteRenderer.enabled = false;
        spriteRenderer.color = Color.white;
    }
}
