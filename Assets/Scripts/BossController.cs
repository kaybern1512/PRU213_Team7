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

        // Kiểm tra an toàn trước khi bắt đầu
        if (player == null) Debug.LogError("Boss chưa có Player trong Inspector!");
        if (groundCheck == null) Debug.LogError("Boss chưa có GroundCheck trong Inspector!");

        // Test: Tự động bắt đầu nếu player tồn tại
        if (player != null) StartBattle();
    }

    void Update()
    {
        if (!isFighting) return;

        // Chỉ thực hiện CheckGround nếu biến groundCheck đã được gán
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
            anim.SetBool("isGrounded", isGrounded);
        }

        // Cập nhật tốc độ rơi vào Animator
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
            // Nghỉ một chút trước khi chọn đòn đánh tiếp theo
            yield return new WaitForSeconds(1.5f);

            int decision = Random.Range(0, 2);
            if (decision == 0)
                yield return StartCoroutine(DashAttack());
            else
                yield return StartCoroutine(SaberCombo());
        }
    }

    void LookAtPlayer()
    {
        if (player == null) return;

        // Tính toán hướng quay mặt dựa trên vị trí Player
        Vector3 localScale = transform.localScale;
        if (player.position.x > transform.position.x)
        {
            localScale.x = Mathf.Abs(localScale.x); // Quay phải
        }
        else
        {
            localScale.x = -Mathf.Abs(localScale.x); // Quay trái
        }
        transform.localScale = localScale;
    }

    IEnumerator DashAttack()
    {
        LookAtPlayer();

        // Giai đoạn chuẩn bị (BeforeDash)
        anim.SetBool("isDashing", true);
        yield return new WaitForSeconds(0.2f);

        // Xác định hướng Dash cố định tại thời điểm bắt đầu
        float direction = (player.position.x > transform.position.x) ? 1f : -1f;

        // Thực hiện lướt trong một khoảng thời gian
        float dashDuration = 0.6f;
        float timer = 0f;
        while (timer < dashDuration)
        {
            // Luôn áp lực di chuyển trong suốt quá trình dash
            rb.linearVelocity = new Vector2(direction * dashSpeed, rb.linearVelocity.y);
            timer += Time.deltaTime;
            yield return null; // Chờ frame tiếp theo
        }

        // Kết thúc Dash
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetBool("isDashing", false);
        yield return new WaitForSeconds(0.3f); // Chờ DashFinish kết thúc
    }

    IEnumerator SaberCombo()
    {
        LookAtPlayer();
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // Nhát chém 1
        anim.SetTrigger("SlashTrigger");
        anim.SetInteger("ComboStep", 1);
        yield return new WaitForSeconds(1.0f);

        // Kiểm tra nếu Player vẫn ở gần thì chém tiếp combo
        if (player != null && Vector2.Distance(transform.position, player.position) < 4f)
        {
            // Nhát chém 2
            anim.SetInteger("ComboStep", 2);
            yield return new WaitForSeconds(0.5f);

            // Nhát chém 3
            anim.SetInteger("ComboStep", 3);
            yield return new WaitForSeconds(1.05f);
        }

        // Trở về trạng thái chờ
        anim.SetInteger("ComboStep", 0);
        yield return new WaitForSeconds(0.25f);
    }
}