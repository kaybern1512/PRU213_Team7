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
        // ta reset cooldown để gây sát thương ngay lập tức
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
            // Kiểm tra cooldown hoặc nếu là Combo thì bỏ qua cooldown cá nhân của BossDamage
            if (Time.time >= nextDamageTime || isComboHit)
            {
                var playerScript = collision.GetComponent<PlayerController>();
                if (playerScript != null)
                {
                    // GỌI HÀM GÂY SỨT THƯƠNG TỪ PLAYER CONTROLLER
                    playerScript.TakeDamage(damage, transform.position);

                    nextDamageTime = Time.time + damageCooldown;
                    isComboHit = false; // Reset sau khi đã gây 1 lần

                    Debug.Log("Boss đã chém trúng Player!");
                }
            }
        }
    }
}
