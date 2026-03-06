using UnityEngine;

public class StaticBackground : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        // Tự động tìm Camera chính
        camTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Ép vị trí của Background luôn trùng với Camera
        // Giữ nguyên trục Z ban đầu của ảnh
        transform.position = new Vector3(camTransform.position.x, camTransform.position.y, transform.position.z);
    }
}