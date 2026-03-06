using UnityEngine;

public class JumpSwitch : MonoBehaviour
{
    public GameObject fireGroup;

    private Animator animator;
    private bool canPress = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canPress) return;

        if (!collision.gameObject.CompareTag("Player") &&
            !collision.gameObject.CompareTag("Box"))
            return;

        ContactPoint2D contact = collision.contacts[0];

        // chỉ khi đè từ trên xuống
        if (contact.normal.y < -0.5f)
        {
            ToggleFire();
            animator.SetBool("Pressed", true);

            canPress = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") &&
            !collision.gameObject.CompareTag("Box"))
            return;

        animator.SetBool("Pressed", false);
        canPress = true;
    }

    void ToggleFire()
    {
        fireGroup.SetActive(!fireGroup.activeSelf);
    }
}