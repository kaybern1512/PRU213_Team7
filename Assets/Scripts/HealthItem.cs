using UnityEngine;

public class HealthItem : MonoBehaviour
{
    public int healAmount = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.Heal(healAmount);
            }

            Destroy(gameObject);
        }
    }
}