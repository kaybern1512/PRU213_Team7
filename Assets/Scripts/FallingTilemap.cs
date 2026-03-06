using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FallingTilemap : MonoBehaviour
{
    public Tilemap fallingTilemap;
    public float triggerDelay = 0.5f;

    [Header("Falling Block Physics")]
    public float gravityScale = 3f;
    public float destroyAfter = 3f;

    private void Reset()
    {
        fallingTilemap = GetComponent<Tilemap>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.collider.CompareTag("Player")) return;

        foreach (var contact in col.contacts)
        {
            Vector3 worldPoint = contact.point + contact.normal * 0.01f;
            Vector3Int cellPos = fallingTilemap.WorldToCell(worldPoint);

            if (fallingTilemap.GetTile(cellPos) == null) continue;

            StartCoroutine(SpawnRuntimeBlockAndRemoveTile(cellPos));
        }
    }

    private IEnumerator SpawnRuntimeBlockAndRemoveTile(Vector3Int cellPos)
    {
        yield return new WaitForSeconds(triggerDelay);

        if (fallingTilemap.GetTile(cellPos) == null) yield break;

        Sprite tileSprite = fallingTilemap.GetSprite(cellPos);

        GameObject block = new GameObject($"FallingBlock_{cellPos.x}_{cellPos.y}");
        block.transform.position = fallingTilemap.GetCellCenterWorld(cellPos);

        var sr = block.AddComponent<SpriteRenderer>();
        sr.sprite = tileSprite;

        var tr = GetComponent<TilemapRenderer>();
        if (tr != null)
        {
            sr.sortingLayerID = tr.sortingLayerID;
            sr.sortingOrder = tr.sortingOrder + 1;
        }

        var box = block.AddComponent<BoxCollider2D>();

        var rb = block.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        rb.freezeRotation = true;

        fallingTilemap.SetTile(cellPos, null);

        Destroy(block, destroyAfter);
    }
}