using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public int health = 100;
    public float moveSpeed = 10f;
    public float jumpForce = 12f;
    public int maxJumpCount = 2;
    public Image healthImage; 

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

    void Start()
    {
        UpdateHealthUI();
    }

    void Update()
    {
        CheckGround();
        HandleJump();
        UpdateAnimation();
    }
    void FixedUpdate()
    {
        HandleMovement();
    }

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

    private void HandleMovement()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );

        if (moveInput != 0)
            spriteRenderer.flipX = moveInput < 0;
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.linearVelocity += Vector2.up * jumpForce;

            jumpCount++;
        }
    }

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
            Debug.Log("HIT DAMAGE: " + collision.name);

            canTakeDamage = false;

            health -= 25;
            UpdateHealthUI();
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UpdateHealthUI()
    {
        if (healthImage != null)
            healthImage.fillAmount = health / 100f;
    }

}
