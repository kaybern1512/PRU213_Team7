using UnityEngine;

public class BossDamage : MonoBehaviour
{
    public int damage = 15;
    public float damageCooldown = 0.5f;
    private float nextDamageTime = 0f;

    // Biến này để xác định đòn hiện tại có phải là nối tiếp combo không
    private bool isComboHit = false;

    public void EnableHitbox(bool isComboStep)
    {
        isComboHit = isComboStep;

        // Nếu là bước tiếp theo của Combo (Chém 2 hoặc 3), 
        // ta reset cooldown để gây sát thương được ngay lập tức
        if (isComboHit)
        {
            nextDamageTime = 0f;
        }

        gameObject.SetActive(true);
    }

    public void DisableHitbox()
    {
        gameObject.SetActive(false);
        isComboHit = false; // Reset trạng thái combo khi tắt hitbox
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Kiểm tra cooldown hoặc nếu là đòn Combo thì bỏ qua cooldown của BossDamage
            if (Time.time >= nextDamageTime || isComboHit)
            {
                var playerScript = collision.GetComponent<PlayerController>();
                if (playerScript != null)
                {
                    // GỌI HÀM GÂY SÁT THƯƠNG THẬT SỰ Ở ĐÂY
                    playerScript.TakeDamage(damage, transform.position);

                    nextDamageTime = Time.time + damageCooldown;
                    isComboHit = false; // Reset sau khi đã trúng 1 nhát

                    Debug.Log("Boss đã chém trúng Player!");
                }
            }
        }
    }
}