using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Player player = collision.GetComponent<Player>();
        player.coins++;

        // ===== TUTORIAL STEP 2 =====
        if (Tutorial.Instance != null && Tutorial.Instance.IsStep(2))
        {
            Tutorial.Instance.CompleteStep(2);
        }

        Destroy(gameObject);
    }

}
