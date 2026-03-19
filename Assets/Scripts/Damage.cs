using UnityEngine;

public class Damage : MonoBehaviour
{
    public int damage = 25;
    public bool knockUp = true;
    public bool instantKill = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerController player = collision.GetComponent<PlayerController>();
        if (player == null) return;

        if (instantKill)
        {
            player.Die();        // chết ngay
        }
        else
        {
            player.TakeDamage(damage, knockUp);
        }
    }
}
