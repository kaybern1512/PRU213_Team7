using UnityEngine;
using Unity.Cinemachine; // Dành cho Cinemachine bản mới (v3)

public class BossRoomTrigger : MonoBehaviour
{
    // Đổi tên từ CinemachineVirtualCamera thành CinemachineCamera
    public CinemachineCamera followCam;
    public CinemachineCamera bossCam;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Trong bản mới, Priority vẫn giữ nguyên cách dùng
            bossCam.Priority = 20;

            Debug.Log("Đã vào phòng Boss! Camera đã chuyển.");
        }
    }
}