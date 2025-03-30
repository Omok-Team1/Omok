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
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void OnMouseEnter()
    {
        spriteRenderer.color = Color.green;
    }

    void OnMouseExit()
    {
        spriteRenderer.color = Color.white;
    }
}
