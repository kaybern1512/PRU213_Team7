using UnityEngine;
using System.Collections;

public class ArenaItemSpawner : MonoBehaviour
{
    public GameObject hpItemPrefab;

    // Nếu script này không nằm cùng object với Collider, 
    // ta nên kéo thả Collider của Arena vào đây từ Inspector.
    public BoxCollider2D arenaCollider;

    [Header("Cài đặt thời gian")]
    public float minDelay = 10f;
    public float maxDelay = 15f;

    [Header("Cài đặt vị trí")]
    [Tooltip("Khoảng cách an toàn để không spawn quá sát mép tường")]
    public float padding = 1f;

    void Start()
    {
        // Nếu chưa kéo vào Inspector, tự động tìm trên cùng Object
        if (arenaCollider == null)
        {
            arenaCollider = GetComponent<BoxCollider2D>();
        }

        if (arenaCollider != null)
        {
            StartCoroutine(SpawnRoutine());
        }
        else
        {
            Debug.LogError("ArenaItemSpawner: Không tìm thấy BoxCollider2D! Hãy kéo Arena Object vào ô Arena Collider.");
        }
    }

    IEnumerator SpawnRoutine()
    {
        // Chờ một chút khi bắt đầu trận đấu rồi mới bắt đầu spawn
        yield return new WaitForSeconds(5f);

        while (true)
        {
            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);

            SpawnInArena();
        }
    }

    void SpawnInArena()
    {
        Bounds bounds = arenaCollider.bounds;

        // Trừ đi phần padding để item không bị dính vào tường/mép Arena
        float randomX = Random.Range(bounds.min.x + padding, bounds.max.x - padding);
        float randomY = Random.Range(bounds.min.y + padding, bounds.max.y - padding);

        Vector3 spawnPos = new Vector3(randomX, randomY, 0);

        // Tạo vật phẩm
        GameObject newItem = Instantiate(hpItemPrefab, spawnPos, Quaternion.identity);

        // Tự hủy sau 8 giây để người chơi có động lực nhặt nhanh và tránh tràn ngập Arena
        Destroy(newItem, 5f);
    }
}