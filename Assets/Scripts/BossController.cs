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
    public GameObject slashTrigger; // Kéo vật chứa Collider của thanh kiếm vào đây

    private bool isGrounded;
    private bool isFighting = false;
    private bool isPerformingAction = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.mass = 1000f;
        rb.gravityScale = 4f;

        // No friction giúp rơi mượt, không dính tường
        PhysicsMaterial2D noFriction = new PhysicsMaterial2D("NoFriction");
        noFriction.friction = 0f;
        noFriction.bounciness = 0f;
        rb.sharedMaterial = noFriction;

        if (slashTrigger != null) slashTrigger.SetActive(false);
        if (player != null) StartBattle();
    }

    void Update()
    {
        if (!isFighting) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("VerticalSpeed", rb.linearVelocity.y);

        // --- CƠ CHẾ CHỐNG TRƯỢT IDLE ---
        if (!isPerformingAction && isGrounded)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            // Khóa vị trí X để không bị trượt
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        }
        else
        {
            // Mở khóa X khi cần Dash, Jump hoặc bị đẩy
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public void StartBattle()
    {
        isFighting = true;
        StartCoroutine(BossLogic());
    }

    // --- ANIMATION EVENTS (Kích hoạt từ Sprite) ---
    public void EnableSlashTrigger() { if (slashTrigger != null) slashTrigger.SetActive(true); }
    public void DisableSlashTrigger() { if (slashTrigger != null) slashTrigger.SetActive(false); }

    IEnumerator BossLogic()
    {
        while (isFighting)
        {
            while (!isGrounded || isPerformingAction) yield return null;

            yield return new WaitForSeconds(attackRate);
            if (player == null) yield break;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (player.position.y > transform.position.y + playerJumpThreshold)
                yield return StartCoroutine(JumpChase());
            else if (distanceToPlayer > attackRange)
                yield return StartCoroutine(DashAttack());
            else
                yield return StartCoroutine(SaberCombo());
        }
    }

    IEnumerator JumpChase()
    {
        isPerformingAction = true;
        LookAtPlayer();

        float direction = (player.position.x > transform.position.x) ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * jumpForceX, jumpForceY);

        yield return new WaitForSeconds(0.2f);

        while (!isGrounded) yield return null;

        // ĐÁP ĐẤT TỨC THÌ
        rb.linearVelocity = Vector2.zero;
        anim.Play("Zero_EnterGround", 0, 0f); // Thay bằng tên state Landing của bạn

        yield return new WaitForSeconds(0.15f); // Thời gian khựng lại khi đáp đất
        isPerformingAction = false;
    }

    IEnumerator DashAttack()
    {
        isPerformingAction = true;
        LookAtPlayer();

        anim.Play("Zero_BeforeDash");
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("isDashing", true);

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

        // Diễn frame kết thúc Dash
        yield return new WaitForSeconds(0.2f);
        isPerformingAction = false;
    }

    IEnumerator SaberCombo()
    {
        isPerformingAction = true;
        LookAtPlayer();
        rb.linearVelocity = Vector2.zero;

        // Chém nhát 1
        anim.Play("Zero_SaberSlash1", 0, 0f);
        anim.SetInteger("ComboStep", 1);
        yield return new WaitForSeconds(0.4f);

        // Chém nhát 2
        if (player != null && Vector2.Distance(transform.position, player.position) < attackRange + 1.5f)
        {
            anim.SetInteger("ComboStep", 2);
            yield return new WaitForSeconds(0.4f);

            // Chém nhát 3
            anim.SetInteger("ComboStep", 3);

            // Khóa Y nếu nhát chém này có sprite lún xuống đất
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
            yield return new WaitForSeconds(0.6f);
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        anim.SetInteger("ComboStep", 0);
        // Chờ diễn nốt frame thu kiếm trước khi cho phép Boss hành động tiếp
        yield return new WaitForSeconds(0.25f);
        isPerformingAction = false;
    }

    void LookAtPlayer()
    {
        if (player == null) return;
        float dir = (player.position.x > transform.position.x) ? 1f : -1f;
        transform.localScale = new Vector3(dir * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}