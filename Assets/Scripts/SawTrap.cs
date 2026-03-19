using UnityEngine;

public class SawTrap : MonoBehaviour
{
    [Header("Movement Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Settings")]
    public float speed = 3f;
    public float rotateSpeed = 300f;

    private Transform target;

    void Start()
    {
        // bắt đầu đi tới pointB
        target = pointB;
    }

    void Update()
    {
        Move();
        RotateSaw();
    }

    void Move()
    {
        // di chuyển tới target
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // nếu gần tới target thì đổi hướng
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            target = (target == pointA) ? pointB : pointA;
        }
    }

    void RotateSaw()
    {
        // quay lưỡi cưa
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
}