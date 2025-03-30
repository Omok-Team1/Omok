using UnityEngine;

public class LoadingRotation : MonoBehaviour
{
    public float rotationSpeed = 100f; // 초당 회전 속도 (도)
    public Vector2 pivotOffset = Vector2.zero; // 회전축 오프셋

    private void Update()
    {
        // 로컬 좌표계에서 회전
        transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime, Space.Self);
    }
}