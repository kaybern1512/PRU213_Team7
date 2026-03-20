using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public int health = 100;
    public float moveSpeed = 14f;
    public float jumpForce = 12f;
    public int maxJumpCount = 2;
    public PlayerAudio playerAudio;
    public int coins = 0;
    public Image healthBar;
    public int maxHealth = 100;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float moveInput;
    private float originalSpeed;
    private bool isSpeedBoosted = false;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private bool isGrounded;
    private int jumpCount;
    private float originalJumpForce;
    private bool isJumpBoosted = false;
    private Animator animator;
    private bool canTakeDamage = true;

    [Header("Fly Settings")]
    public float flyMoveSpeed = 10f;
    public float flyVerticalSpeed = 10f;
    public GameObject headFan;

    private bool isFlying = false;
    private float originalGravityScale;
    private Coroutine flyCoroutine;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalJumpForce = jumpForce;
        originalSpeed = moveSpeed;
        originalGravityScale = rb.gravityScale;

        if (headFan != null)
            headFan.SetActive(false);
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (isFlying)
        {
            HandleFlyMovement();
            UpdateAnimation();
            return;
        }

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
            groundCheckRadius,   
            groundLayer
        );

        if (isGrounded)
        {
            jumpCount = 0;
        }
    }

    // ================= DI CHUYỂN =================
    private void HandleMovement()
    {
        //moveInput = Input.GetAxis("Horizontal");

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
        if (isFlying)
        {
            animator.SetBool("isFlying", true);
            animator.SetBool("isGrounded", false);
            animator.SetBool("isRunning", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
            animator.SetFloat("yVelocity", rb.linearVelocity.y);
            return;
        }

        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetBool("isFlying", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Damage"))
        {
            TakeDamage(25, true);
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

    public void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        if (health > maxHealth)
            health = maxHealth;

        if (healthBar != null)
        {
            healthBar.fillAmount = (float)health / maxHealth;
        }
    }

    public void TakeDamage(int damage, bool knockUp)
    {
        if (!canTakeDamage) return;

        canTakeDamage = false;

        health -= damage;

        if (healthBar != null)
        {
            healthBar.fillAmount = (float)health / maxHealth;
        }

        if (playerAudio != null)
            playerAudio.PlayHurt();

        if (knockUp)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        StartCoroutine(BLinkRed());
        Invoke(nameof(ResetCanTakeDamage), 0.4f);

        if (health <= 0)
            Die();
    }
    private void ResetCanTakeDamage()
    {
        canTakeDamage = true;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
    }
    public void BoostSpeed(float multiplier, float duration)
    {
        if (isSpeedBoosted) return;
        isSpeedBoosted = true;
        moveSpeed *= multiplier;
        Invoke(nameof(ResetSpeed), duration);
    }

    private void ResetSpeed()
    {
        moveSpeed = originalSpeed;
        isSpeedBoosted = false;
    }

    private void HandleFlyMovement()
    {
        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
            verticalInput = 1f;
        else if (Input.GetKey(KeyCode.S))
            verticalInput = -1f;

        rb.linearVelocity = new Vector2(
            moveInput * flyMoveSpeed,
            verticalInput * flyVerticalSpeed
        );

        if (moveInput != 0)
            spriteRenderer.flipX = moveInput < 0;
    }

    public void ActivateFly(float duration)
    {
        if (flyCoroutine != null)
            StopCoroutine(flyCoroutine);

        flyCoroutine = StartCoroutine(FlyCoroutine(duration));
    }

    private IEnumerator FlyCoroutine(float duration)
    {
        isFlying = true;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        if (headFan != null)
            headFan.SetActive(true);

        if (animator != null)
            animator.SetBool("isFlying", true);

        yield return new WaitForSeconds(duration);

        isFlying = false;
        rb.gravityScale = originalGravityScale;

        if (headFan != null)
            headFan.SetActive(false);

        if (animator != null)
            animator.SetBool("isFlying", false);

        flyCoroutine = null;
    }
}

