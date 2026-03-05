using System.Collections;
using UnityEngine;

public class Scene3PlayerTuning : MonoBehaviour
{
    [Header("Scene 3 Player Stats")]
    public float scene3MoveSpeed = 6f;
    public float scene3JumpForce = 6f;   
    public int scene3MaxJump = 2;

    [Header("Physics Feel (per Scene 3)")]
    public float gravityScale = 3.2f;       
    public float fallMultiplier = 1.8f;     
    public float lowJumpMultiplier = 1.4f;  

    [Header("Optional: clamp max horizontal velocity")]
    public bool clampXVelocity = true;
    public float maxXVelocity = 9.0f;

    private PlayerController pc;
    private Rigidbody2D rb;

    IEnumerator Start()
    {
        yield return null;
        yield return null;
        FindAndApply();
    }

    void FindAndApply()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) player = GameObject.Find("Player");

        if (player == null)
        {
            Debug.LogWarning("[Scene3PlayerTuning] Cannot find Player yet.");
            return;
        }

        pc = player.GetComponent<PlayerController>();
        rb = player.GetComponent<Rigidbody2D>();

        if (pc == null || rb == null)
        {
            Debug.LogWarning("[Scene3PlayerTuning] Missing PlayerController or Rigidbody2D.");
            return;
        }

        // Stats riêng cho Scene3
        pc.moveSpeed = scene3MoveSpeed;
        pc.jumpForce = scene3JumpForce;
        pc.maxJumpCount = scene3MaxJump;

        // Physics riêng cho Scene3
        rb.gravityScale = gravityScale;

        Debug.Log("[Scene3PlayerTuning] Applied Scene3 stats + physics.");
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Clamp tốc độ chạy
        if (clampXVelocity)
        {
            Vector2 v = rb.linearVelocity;
            v.x = Mathf.Clamp(v.x, -maxXVelocity, maxXVelocity);
            rb.linearVelocity = v;
        }

        // ✅ Làm rơi nhanh hơn + nhảy “đã tay” (không floaty)
        // rơi xuống: tăng gravity
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        // đang bay lên nhưng thả nút Jump sớm: giảm độ cao (nhảy nhấp nhả)
        else if (rb.linearVelocity.y > 0f && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }
}