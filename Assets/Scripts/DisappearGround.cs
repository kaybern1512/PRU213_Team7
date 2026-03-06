using UnityEngine;
using UnityEngine.Tilemaps;

public class DisappearTilemap : MonoBehaviour
{
    [Header("Settings")]
    public float blinkDuration = 4f;
    public float blinkInterval = 0.15f;

    private TilemapRenderer tr;
    private TilemapCollider2D tileCol;
    private CompositeCollider2D compositeCol;
    private bool triggered = false;

    void Start()
    {
        tr = GetComponent<TilemapRenderer>();

        tileCol = GetComponent<TilemapCollider2D>();
        compositeCol = GetComponent<CompositeCollider2D>();

        // Nếu dùng Composite → tắt collider gốc để tránh lỗi
        if (tileCol != null && tileCol.usedByComposite)
        {
            tileCol.enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (triggered) return;

        if (collision.collider.CompareTag("Player") ||
            collision.collider.CompareTag("Enemy"))
        {
            triggered = true;
            StartCoroutine(BlinkAndDisappear());
        }
    }

    private System.Collections.IEnumerator BlinkAndDisappear()
    {
        float timer = 0f;

        while (timer < blinkDuration)
        {
            tr.enabled = !tr.enabled; // Nhấp nháy toàn bộ tilemap
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        // Biến mất hoàn toàn
        tr.enabled = false;

        if (compositeCol != null)
            compositeCol.enabled = false;

        if (tileCol != null && !tileCol.usedByComposite)
            tileCol.enabled = false;
    }
}