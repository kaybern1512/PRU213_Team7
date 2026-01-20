using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpForce = 12f;
    public int maxJumpCount = 2;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float moveInput;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    private bool isGrounded;
    private int jumpCount;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        CheckGround();
        HandleMovement();
        HandleJump();
        UpdateAnimation();
    }

    // ================= KIỂM TRA CHẠM ĐẤT =================
    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            0.12f,
            groundLayer
        );

        if (isGrounded)
        {
            jumpCount = 0; // Reset số lần nhảy khi chạm đất
        }
    }

    // ================= DI CHUYỂN =================
    private void HandleMovement()
    {
        moveInput = Input.GetAxis("Horizontal");

        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );

        if (moveInput != 0)
            spriteRenderer.flipX = moveInput < 0;
    }

    // ================= NHẢY (DOUBLE JUMP) =================
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );
            jumpCount++;
        }
    }

    // ================= ANIMATION =================
    private void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }
}
