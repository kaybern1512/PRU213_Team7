using UnityEngine;

public class BossTriggerZone : MonoBehaviour
{
    public BossController boss; // Kéo Boss vào ô này trong Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu là Player chạm vào vùng kích hoạt
        if (other.CompareTag("Player"))
        {
            if (boss != null)
            {
                boss.StartBattle();
                // Hủy hoặc tắt trigger sau khi kích hoạt để tránh gọi lại nhiều lần
                gameObject.SetActive(false);
            }
        }
    }
}