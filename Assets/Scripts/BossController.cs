using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;

    [Header("Settings")]
    public float dashSpeed = 15f;
    public Transform player;
    public LayerMask groundLayer;
    public Transform groundCheck;

    private bool isGrounded;
    private bool isFighting = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isFighting) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("VerticalSpeed", rb.linearVelocity.y);
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
            yield return new WaitForSeconds(1.2f); // Nghỉ giữa các đợt tấn công

            int decision = Random.Range(0, 2);
            if (decision == 0) yield return StartCoroutine(DashAttack());
            else yield return StartCoroutine(SaberCombo());
        }
    }

    IEnumerator DashAttack()
    {
        LookAtPlayer();

        // 1. Chạy BeforeDash (0.2s theo bạn cung cấp)
        anim.SetBool("isDashing", true);
        yield return new WaitForSeconds(0.2f);

        // 2. Bắt đầu đẩy lực lướt (Dashing)
        float dir = (player.position.x > transform.position.x) ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * dashSpeed, rb.linearVelocity.y);

        yield return new WaitForSeconds(0.6f); // Thời gian Zero trượt trên mặt đất

        // 3. Dừng và chạy DashFinish
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isDashing", false);

        // Chờ một chút để DashFinish diễn ra (thường khoảng 0.2s - 0.3s)
        yield return new WaitForSeconds(0.25f);
    }

    IEnumerator SaberCombo()
    {
        LookAtPlayer();
        rb.linearVelocity = Vector2.zero;

        // Nhát chém 1 (1.0s)
        anim.SetTrigger("SlashTrigger");
        anim.SetInteger("ComboStep", 1);
        yield return new WaitForSeconds(1.0f);

        // Nhát chém 2 (0.5s)
        if (Vector2.Distance(transform.position, player.position) < 4f)
        {
            anim.SetInteger("ComboStep", 2);
            yield return new WaitForSeconds(0.5f);

            // Nhát chém 3 (1.05s)
            anim.SetInteger("ComboStep", 3);
            yield return new WaitForSeconds(1.05f);
        }

        // Kết thúc: Chuyển về SlashEnd (0.25s)
        anim.SetInteger("ComboStep", 0);
        yield return new WaitForSeconds(0.25f);

        // Sau 0.25s của SlashEnd, Animator sẽ tự về Idle nhờ Has Exit Time
    }

    void LookAtPlayer()
    {
        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }
}