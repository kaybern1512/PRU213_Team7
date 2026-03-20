using UnityEngine;

public class Fan : MonoBehaviour
{
    public float flyDuration = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player picked fan");

            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ActivateFly(flyDuration);
            }

            Destroy(gameObject);
        }
    }
}