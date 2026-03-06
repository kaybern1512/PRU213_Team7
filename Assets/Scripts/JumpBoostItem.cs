using UnityEngine;

public class JumpBoostItem : MonoBehaviour
{
    public float multiplier = 1.2f;
    public float duration = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.BoostJump(multiplier, duration);
            }

            Destroy(gameObject);
        }
    }
}