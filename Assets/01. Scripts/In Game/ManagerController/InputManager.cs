using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    public static bool GetInput()
    {
        return Input.GetMouseButtonUp(0);
    }

    public static RaycastHit2D TryRaycastHit()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        
        return Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity);
    }
    
    public static RaycastHit2D TryRaycastHit(string targetLayer)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        return Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, 1 << LayerMask.NameToLayer(targetLayer));
    }
}
