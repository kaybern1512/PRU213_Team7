using UnityEngine;

public class SawMove : MonoBehaviour
{
    public Transform point1;
    public Transform point2;
    public float speed = 3f;
    public int damage = 25;

    private Transform target;
    private Animator anim;

    void Start()
    {
        target = point2;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            if (target == point1)
            {
                target = point2;
                anim.SetBool("Right", true);
            }
            else
            {
                target = point1;
                anim.SetBool("Right", false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(damage, true);
        }
    }
}