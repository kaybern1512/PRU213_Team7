using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public Transform[] points;
    public int damage = 25;
    public bool instantKill = false;

    private int i;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 2f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 target = points[i].position;
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);

        rb.MovePosition(newPos);

        if (Vector2.Distance(rb.position, target) < 0.25f)
        {
            i++;
            if (i >= points.Length)
                i = 0;
        }

        spriteRenderer.flipX = (rb.position.x - points[i].position.x) < 0f;
    }
    void Die()
    {
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null) return;

        ContactPoint2D contact = collision.contacts[0];

        // player rơi từ trên xuống
        if (contact.normal.y < -0.5f)
        {
            Die();  // enemy chết

            // bật player lên nhẹ giống Mario
            Rigidbody2D prb = collision.gameObject.GetComponent<Rigidbody2D>();
            prb.linearVelocity = new Vector2(prb.linearVelocity.x, 10f);
        }
        else
        {
            player.TakeDamage(damage, true); // player mất máu
        }
    }
}
