using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public int extraJumpsValue = 1;
    public int health = 100;
    public Image healthImage;
    public int coins;

    private Rigidbody2D rb;
    private PlayerInputActions input;
    private bool isGrounded;
    private float moveInput;
    private Animator animator;
    private int extraJumps;
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        extraJumps = extraJumpsValue;

        //input = new PlayerInputActions();
        //input.Player.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        // Ground check 
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // Move
        Vector2 move = input.Player.Move.ReadValue<Vector2>();
        moveInput = move.x;

        // ===== TUTORIAL STEP 0 =====
        if (Tutorial.Instance != null && Tutorial.Instance.IsStep(0))
        {
            if (Mathf.Abs(moveInput) > 0.1f)
            {
                Tutorial.Instance.CompleteStep(0);
            }
        }


        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        //Double jump
        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
        }

        // Jump
        if (input.Player.Jump.triggered)
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

                // ===== TUTORIAL STEP 1 =====
                if (Tutorial.Instance != null && Tutorial.Instance.IsStep(1))
                {
                    Tutorial.Instance.CompleteStep(1);
                }

            }
            else if (extraJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                extraJumps--;
            }
        }

        SetAnimation(moveInput);

        healthImage.fillAmount = health / 100f;
    }

    private void SetAnimation(float moveInput)
    {
        if (isGrounded)
        {
            if (moveInput != 0)
            {
                animator.Play("Player_run");
            }
            else
            {
                animator.Play("Player_idle");
            }
        }
        else
        {
            if (rb.linearVelocity.y > 0)
            {
                animator.Play("Player_jump");
            }
            else
            {
                animator.Play("Player_fall");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Damage"))
        {
            health -= 25;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            StartCoroutine(Blinked());
            if (health <= 0)
            {
                Die();
            }
        }
    }

    private IEnumerator Blinked()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;

    }
    private void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Player.Enable();
    }

    void OnDisable()
    {
        input.Player.Disable();
    }
}
