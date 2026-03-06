using UnityEngine;

public class FireTrap : MonoBehaviour
{
    public bool isActive = true;

    public Collider2D damageCollider;

    public Sprite fireOff;     // sprite khi tắt
    private SpriteRenderer sr;

    private Animator animator; // animator cho lửa

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
}