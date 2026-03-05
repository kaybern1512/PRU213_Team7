using UnityEngine;

public class CameraChaseScene3 : MonoBehaviour
{
    public float smoothSpeed = 6f;
    public float xOffset = 3.5f;   // dương => player nằm bên phải
    public float yOffset = 1.5f;

    public bool onlyMoveRight = true;
    private float minX;

    private Transform target;

    void Start()
    {
        FindPlayer();
        minX = transform.position.x;
    }

    void LateUpdate()
    {
        if (target == null) { FindPlayer(); return; }

        float desiredX = target.position.x + xOffset;
        float desiredY = target.position.y + yOffset;

        if (onlyMoveRight) desiredX = Mathf.Max(minX, desiredX);

        Vector3 desiredPos = new Vector3(desiredX, desiredY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        if (onlyMoveRight) minX = transform.position.x;
    }

    void FindPlayer()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) target = p.transform;
    }
}