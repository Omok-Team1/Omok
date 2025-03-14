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
        Debug.Log("Mouse enter");
        spriteRenderer.color = Color.green;
    }

    void OnMouseExit()
    {
        Debug.Log("Mouse Exit");
        spriteRenderer.color = Color.white;
    }
}
