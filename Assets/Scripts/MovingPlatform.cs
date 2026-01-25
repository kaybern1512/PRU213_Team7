using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 2f;
    public Transform[] points;

    private Vector3[] worldPoints; // lưu vị trí world của point
    private int i;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // kiểm tra an toàn
        if (points == null || points.Length < 2)
        {
            Debug.LogError($"{gameObject.name} cần ít nhất 2 point");
            enabled = false;
            return;
        }

        // lưu vị trí world của các point (tránh point bị kéo theo platform)
        worldPoints = new Vector3[points.Length];
        for (int j = 0; j < points.Length; j++)
        {
            worldPoints[j] = points[j].position;
        }

        transform.position = worldPoints[0];
        i = 1; // bắt đầu đi tới point tiếp theo
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, worldPoints[i]) < 0.01f)
        {
            i++;
            if (i == worldPoints.Length)
            {
                i = 0;
            }
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            worldPoints[i],
            speed * Time.deltaTime
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform, true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null, true);
        }
    }
}
