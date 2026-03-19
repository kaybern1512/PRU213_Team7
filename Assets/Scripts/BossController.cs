using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;

    [Header("Settings")]
    public float dashSpeed = 22f;
    public float attackRange = 5.5f;
    public float attackRate = 0.5f;

    [Header("Jump Settings")]
    public float jumpForceX = 12f;
    public float jumpForceY = 16f;
    public float playerJumpThreshold = 2f;

    [Header("References")]
    public Transform player;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public GameObject slashTrigger;

    // Tham chiếu đến script BossDamage để điều khiển logic gây dmg
    private BossDamage bossDamage;

    private bool isGrounded;
    private bool isFighting = false;
    private bool isPerformingAction = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Lấy script BossDamage từ slashTrigger
        if (slashTrigger != null)
            bossDamage = slashTrigger.GetComponent<BossDamage>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.mass = 1000f;
        rb.gravityScale = 4f;

        PhysicsMaterial2D noFriction = new PhysicsMaterial2D("NoFriction");
        noFriction.friction = 0f;
        noFriction.bounciness = 0f;
        rb.sharedMaterial = noFriction;

        if (slashTrigger != null) slashTrigger.SetActive(false);
    }

    void Update()
    {
        if (!isFighting) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("VerticalSpeed", rb.linearVelocity.y);

        if (!isPerformingAction && isGrounded)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        }
        else if (!isPerformingAction)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    // --- HÀM BẬT TẮT HITBOX MỚI ---
    // isCombo: true nếu muốn bỏ qua iframe của player (dành cho chém 2, 3)
    public void EnableSlash(bool isCombo)
    {
        if (slashTrigger != null && bossDamage != null)
        {
            bossDamage.EnableHitbox(isCombo);
        }
    }

    public void DisableSlash()
    {
        if (slashTrigger != null && bossDamage != null)
        {
            bossDamage.DisableHitbox();
        }
    }

    // Giữ lại tên cũ để tránh lỗi nếu bạn đã gán vào Animation Event
    public void EnableSlashTrigger() { EnableSlash(false); }
    public void DisableSlashTrigger() { DisableSlash(); }

    public void StartBattle()
    {
        if (isFighting) return;
        StartCoroutine(WaitForPlayerAndStart());
    }

    IEnumerator WaitForPlayerAndStart()
    {
        // Kiểm tra nếu biến player vẫn trống (do chưa gán từ Inspector hoặc Trigger)
        if (player == null)
        {
            GameObject playerObj = null;

            // Lặp cho đến khi tìm thấy Player có Tag
            while (playerObj == null)
            {
                playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj == null)
                {
                    Debug.Log("Boss đang đợi Player spawn...");
                    yield return new WaitForSeconds(0.2f); // Đợi một chút rồi tìm tiếp
                }
            }
            player = playerObj.transform;
        }

        isFighting = true;

        if (StageMusicManager.instance != null)
            StageMusicManager.instance.PlayBossMusic();

        StartCoroutine(BossLogic());
        Debug.Log("Boss đã tìm thấy Player và bắt đầu chiến đấu!");
    }

    IEnumerator BossLogic()
    {
        while (isFighting)
        {
            while (!isGrounded || isPerformingAction) yield return null;
            yield return new WaitForSeconds(attackRate);
            if (player == null) yield break;

            // Tính khoảng cách theo trục X để chính xác hơn cho game 2D side-scrolling
            float distanceX = Mathf.Abs(transform.position.x - player.position.x);

            // 1. Ưu tiên nhảy đuổi nếu người chơi ở trên cao
            if (player.position.y > transform.position.y + playerJumpThreshold)
            {
                yield return StartCoroutine(JumpChase());
            }
            // 2. Ưu tiên CHÉM nếu đã vào tầm attackRange
            else if (distanceX <= attackRange)
            {
                yield return StartCoroutine(SaberCombo());
            }
            // 3. Nếu ở xa quá thì mới DASH áp sát
            else
            {
                yield return StartCoroutine(DashAttack());
            }
        }
    }

    IEnumerator JumpChase()
    {
        isPerformingAction = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        LookAtPlayer();
        float direction = (player.position.x > transform.position.x) ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * jumpForceX, jumpForceY);
        yield return new WaitForSeconds(0.2f);
        while (!isGrounded) yield return null;
        rb.linearVelocity = Vector2.zero;
        anim.Play("Zero_EnterGround", 0, 0f);
        yield return new WaitForSeconds(0.15f);
        isPerformingAction = false;
    }

    IEnumerator DashAttack()
    {
        isPerformingAction = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        LookAtPlayer();
        anim.Play("Zero_BeforeDash");
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("isDashing", true);

        // Dash cũng gây damage nhưng có iframe (không phải combo)
        EnableSlash(false);

        float direction = (player.position.x > transform.position.x) ? 1f : -1f;
        float dashTimer = 0.6f;
        while (dashTimer > 0)
        {
            if (player == null || Vector2.Distance(transform.position, player.position) < 1.2f) break;
            rb.linearVelocity = new Vector2(direction * dashSpeed, rb.linearVelocity.y);
            dashTimer -= Time.deltaTime;
            yield return null;
        }

        DisableSlash();
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isDashing", false);
        yield return new WaitForSeconds(0.2f);
        isPerformingAction = false;
    }

    IEnumerator SaberCombo()
    {
        isPerformingAction = true;
        LookAtPlayer();
        rb.linearVelocity = Vector2.zero;

        // --- ĐÒN 1: Bình thường (Player có iframe) ---
        anim.Play("Zero_SaberSlash1", 0, 0f);
        anim.SetInteger("ComboStep", 1);
        EnableSlash(false);
        yield return new WaitForSeconds(0.4f);
        DisableSlash();

        if (player != null && Vector2.Distance(transform.position, player.position) < attackRange + 1.5f)
        {
            // --- ĐÒN 2: COMBO (Bỏ qua iframe nếu đòn 1 vừa trúng) ---
            anim.SetInteger("ComboStep", 2);
            EnableSlash(true);
            yield return new WaitForSeconds(0.4f);
            DisableSlash();

            // --- ĐÒN 3: COMBO ---
            anim.SetInteger("ComboStep", 3);
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezePositionX;
            EnableSlash(true);
            yield return new WaitForSeconds(0.6f);
            DisableSlash();
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        anim.SetInteger("ComboStep", 0);
        yield return new WaitForSeconds(0.25f);
        isPerformingAction = false;
    }

    void LookAtPlayer()
    {
        if (player == null) return;
        float dir = (player.position.x > transform.position.x) ? 1f : -1f;
        transform.localScale = new Vector3(dir * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    // Vẽ Gizmos để bạn dễ kiểm tra GroundCheck trong Scene
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }
    }
}