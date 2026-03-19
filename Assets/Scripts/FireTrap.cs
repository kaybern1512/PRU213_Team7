using UnityEngine;

public class FireTrap : MonoBehaviour
{
    public bool isActive = true;

    public Collider2D damageCollider;

    public Sprite fireOff;
    private SpriteRenderer sr;

    private Animator animator;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void SetFire(bool state)
    {
        isActive = state;

        damageCollider.enabled = state;

        animator.SetBool("isOn", state);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;

        PlayerController player = collision.GetComponent<PlayerController>();

        if (player != null)
        {
            player.TakeDamage(100, true); // hard code damage 100
        }
    }
}