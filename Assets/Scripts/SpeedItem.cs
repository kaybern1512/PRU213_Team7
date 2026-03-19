using UnityEngine;

public class Speeditem : MonoBehaviour
{
    public float speedMultiplier = 1.5f; // nhanh 1.5 l?n
    public float duration = 5f;          // 5 giây

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                player.BoostSpeed(speedMultiplier, duration);
            }

            Destroy(gameObject); // ?n xong bi?n m?t
        }
    }
}