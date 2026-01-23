using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public float smoothSpeed = 5f;
    public float yOffset = 1.5f;

    private Transform target;
    private float fixedY;

    void Start()
    {
        fixedY = transform.position.y;
        FindPlayer();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindPlayer();
            return;
        }

        Vector3 desiredPos = new Vector3(
            target.position.x,
            fixedY + yOffset,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            smoothSpeed * Time.deltaTime
        );
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            target = player.transform;
    }
}
