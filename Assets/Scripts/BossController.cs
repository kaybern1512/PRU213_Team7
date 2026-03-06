using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;

    [Header("Settings")]
    public float dashSpeed = 22f;
    public float attackRange = 3.5f;
    public float attackRate = 0.5f;

    [Header("Jump Settings")]
    public float jumpForceX = 12f;
    public float jumpForceY = 16f;
    public float playerJumpThreshold = 2f;

    [Header("References")]
    public Transform player;
    public LayerMask groundLayer;
    public Transform groundCheck;

    private bool isGrounded;
    private bool isFighting = false;
    private bool isPerformingAction = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.mass = 1000f; // Giảm mass xuống mức hợp lý hơn (1000 là đủ)
        rb.gravityScale = 4f;

        // Tạo vật lý không ma sát để không bị dính vào tường
        PhysicsMaterial2D noFriction = new PhysicsMaterial2D("NoFriction");
        noFriction.friction = 0f;
        noFriction.bounciness = 0f;
        rb.sharedMaterial = noFriction;

        if (player != null) StartBattle();
    }

    void Update()
    {
        if (!isFighting) return;

        // Kiểm tra đất bằng Circle kết hợp khoảng cách nhỏ
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("VerticalSpeed", rb.linearVelocity.y);

        // Chống trượt trên mặt đất khi không làm gì
        if (!isPerformingAction && isGrounded)
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
            // ĐIỀU KIỆN TIÊN QUYẾT: Boss phải ở trên đất mới được tính toán đòn tiếp theo
            while (!isGrounded || isPerformingAction)
            {
                yield return null;
            }

            yield return new WaitForSeconds(attackRate);

            if (player == null) yield break;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Ưu tiên nhảy nếu player nhảy cao
            if (player.position.y > transform.position.y + playerJumpThreshold)
            {
                yield return StartCoroutine(JumpChase());
            }
            else if (distanceToPlayer > attackRange)
            {
                yield return StartCoroutine(DashAttack());
            }
            else
            {
                yield return StartCoroutine(SaberCombo());
            }
        }
    }

    IEnumerator JumpChase()
    {
        isPerformingAction = true;
        LookAtPlayer();

        float direction = (player.position.x > transform.position.x) ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * jumpForceX, jumpForceY);

        // Buộc phải rời đất trước khi bắt đầu check chạm đất lại
        yield return new WaitForSeconds(0.3f);

        // Đợi chạm đất (với timeout để tránh kẹt)
        float timer = 0;
        while (!isGrounded && timer < 3f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        isPerformingAction = false;
    }

    IEnumerator DashAttack()
    {
        isPerformingAction = true;
        LookAtPlayer();

        anim.Play("Zero_BeforeDash");
        anim.SetBool("isDashing", true);
        yield return new WaitForSeconds(0.1f);

        float direction = (player.position.x > transform.position.x) ? 1f : -1f;
        float dashTimer = 0.6f;

        while (dashTimer > 0)
        {
            if (Vector2.Distance(transform.position, player.position) < 1.2f) break;

            rb.linearVelocity = new Vector2(direction * dashSpeed, rb.linearVelocity.y);
            dashTimer -= Time.deltaTime;
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

        anim.Play("Zero_SaberSlash1");
        anim.SetInteger("ComboStep", 1);
        yield return new WaitForSeconds(0.4f);

        if (player != null && Vector2.Distance(transform.position, player.position) < attackRange + 1.5f)
        {
            anim.SetInteger("ComboStep", 2);
            yield return new WaitForSeconds(0.4f);
            anim.SetInteger("ComboStep", 3);
            yield return new WaitForSeconds(0.5f);
        }

        anim.SetInteger("ComboStep", 0);
        yield return new WaitForSeconds(0.2f); // Nghỉ sau combo
        isPerformingAction = false;
    }

    void LookAtPlayer()
    {
        if (player == null) return;
        float dir = (player.position.x > transform.position.x) ? 1f : -1f;
        transform.localScale = new Vector3(dir * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}