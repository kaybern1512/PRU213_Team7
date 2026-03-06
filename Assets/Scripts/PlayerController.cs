using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public int health = 100;
    public float moveSpeed = 14f;
    public float jumpForce = 12f;
    public int maxJumpCount = 2;
    public PlayerAudio playerAudio;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float moveInput;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    private bool isGrounded;
    private int jumpCount;
    private float originalJumpForce;
    private bool isJumpBoosted = false;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalJumpForce = jumpForce;

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
            if (playerAudio != null)
                playerAudio.PlayJump();
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


    private bool canTakeDamage = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Damage") && canTakeDamage)
        {

            canTakeDamage = false;

            health -= 25;
            if (playerAudio != null)
                playerAudio.PlayHurt();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            StartCoroutine(BLinkRed());

            if (health <= 0)
                Die();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Damage"))
            canTakeDamage = true;
    }
    private IEnumerator BLinkRed()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BoostJump(float multiplier, float duration)
    {
        if (isJumpBoosted) return;

        isJumpBoosted = true;
        jumpForce *= multiplier;
        Invoke(nameof(ResetJump), duration);
    }

    private void ResetJump()
    {
        jumpForce = originalJumpForce;
        isJumpBoosted = false;
    }

    public void Heal(int amount)
    {
        health += amount;

        if (health > 100)
            health = 100;

        Debug.Log("Ăn item hồi máu +" + amount +
                  " | HP hiện tại: " + health);
    }
}
