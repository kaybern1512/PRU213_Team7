using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Patrol")]
    public Transform point1;
    public Transform point2;
    public float speed = 2f;

    [Header("Combat")]
    public int damage = 25;
    public float bounceForce = 8f;

    [Header("Drop")]
    public GameObject coinPrefab;
    public Transform coinSpawnPoint;
    public int coinCount = 5;
    public float spacing = 0.6f;

    private bool isDead = false;
    private Transform target;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        target = point2;
    }

    void FixedUpdate()
    {
        if (isDead) return;
        if (point1 == null || point2 == null) return;

        Vector2 newPos = Vector2.MoveTowards(
            rb.position,
            target.position,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPos);

        if (Vector2.Distance(rb.position, target.position) < 0.05f)
        {
            if (target == point1)
                target = point2;
            else
                target = point1;
        }

        if (sr != null)
        {
            sr.flipX = target.position.x > transform.position.x;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        HandleCollision(collision);
    }

    private void HandleCollision(Collision2D collision)
    {
        if (isDead) return;
        if (!collision.gameObject.CompareTag("Player")) return;
        if (collision.contactCount == 0) return;

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (player == null || playerRb == null) return;

        ContactPoint2D contact = collision.GetContact(0);

        // Đạp từ trên xuống -> enemy chết
        if (contact.normal.y < -0.5f && playerRb.linearVelocity.y < -0.1f)
        {
            Die();
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);
            return;
        }

        // Chạm ngang -> player mất máu
        if (Mathf.Abs(contact.normal.x) > 0.5f)
        {
            player.TakeDamage(damage, true);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Vector3 center = coinSpawnPoint != null ? coinSpawnPoint.position : transform.position;
        float totalWidth = (coinCount - 1) * spacing;

        for (int i = 0; i < coinCount; i++)
        {
            float offsetX = i * spacing - totalWidth / 2f;
            Vector3 spawnPos = center + new Vector3(offsetX, 0f, 0f);
            Instantiate(coinPrefab, spawnPos, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}