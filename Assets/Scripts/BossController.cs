using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;

    [Header("Settings")]
    public float dashSpeed = 22f;    // Tăng tốc độ để ra đòn gắt hơn
    public float attackRange = 3.5f; // Khoảng cách để bắt đầu chém
    public float attackRate = 0.5f;  // Nghỉ giữa các đòn đánh

    [Header("References")]
    public Transform player;
    public LayerMask groundLayer;    // Phải chọn đúng Layer sàn nhà trong Inspector
    public Transform groundCheck;

    private bool isGrounded;
    private bool isFighting = false;
    private bool isPerformingAction = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Thiết lập vật lý: Boss cực nặng để không vật thể nào đẩy được
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.mass = 10000f;

        // Đảm bảo Boss không bị ma sát làm chậm khi Dash
        rb.sharedMaterial = new PhysicsMaterial2D { friction = 0f };

        if (player != null) StartBattle();
    }

    void Update()
    {
        if (!isFighting) return;

        // Kiểm tra mặt đất - Đây là mấu chốt để animation không bị "liệt"
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("VerticalSpeed", rb.linearVelocity.y);

        // CHỐNG TRƯỢT KHI IDLE: Ép vận tốc về 0 nếu không trong hành động
        if (!isPerformingAction)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    public void StartBattle()
    {
        isFighting = true;
        StartCoroutine(BossLogic());
    }

    IEnumerator BossLogic()
    {
        while (isFighting)
        {
            isPerformingAction = false;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            yield return new WaitForSeconds(attackRate);

            if (player == null) yield break;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // LOGIC ƯU TIÊN: Tiếp cận rồi mới chém
            if (distanceToPlayer > attackRange)
            {
                yield return StartCoroutine(DashAttack());
            }
            else
            {
                yield return StartCoroutine(SaberCombo());
            }
        }
    }

    void LookAtPlayer()
    {
        if (player == null) return;
        float dir = (player.position.x > transform.position.x) ? 1f : -1f;
        transform.localScale = new Vector3(dir * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    IEnumerator DashAttack()
    {
        isPerformingAction = true;
        LookAtPlayer();

        // ÉP CHẠY ANIMATION DASH (Thay "Dash" bằng tên chính xác trong Animator của bạn)
        anim.Play("Zero_BeforeDash");
        anim.SetBool("isDashing", true);

        yield return new WaitForSeconds(0.05f); // Chờ cực ngắn để tạo cảm giác phản ứng nhanh

        float direction = (player.position.x > transform.position.x) ? 1f : -1f;
        float timer = 0.6f;

        while (timer > 0)
        {
            // Zero đi xuyên qua player và chỉ dừng khi cách 1 khoảng nhỏ hoặc hết thời gian
            float currentDist = Vector2.Distance(transform.position, player.position);
            if (currentDist < 1.0f) break;

            rb.linearVelocity = new Vector2(direction * dashSpeed, rb.linearVelocity.y);
            timer -= Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isDashing", false);
        yield return new WaitForSeconds(0.1f);
        isPerformingAction = false;
    }

    IEnumerator SaberCombo()
    {
        isPerformingAction = true;
        LookAtPlayer();
        rb.linearVelocity = Vector2.zero;

        // Nhát 1 - Ép chạy animation chém ngay lập tức
        anim.Play("Zero_SaberSlash1");
        anim.SetTrigger("SlashTrigger");
        anim.SetInteger("ComboStep", 1);
        yield return new WaitForSeconds(0.4f);

        // Kiểm tra nếu player vẫn trong tầm đánh thì chém tiếp
        if (player != null && Vector2.Distance(transform.position, player.position) < attackRange + 1.5f)
        {
            anim.SetInteger("ComboStep", 2);
            yield return new WaitForSeconds(0.4f);

            anim.SetInteger("ComboStep", 3);
            yield return new WaitForSeconds(0.5f);
        }

        anim.SetInteger("ComboStep", 0);
        yield return new WaitForSeconds(0.1f);
        isPerformingAction = false;
    }
}