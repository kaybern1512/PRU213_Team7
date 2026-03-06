using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollowZoomScene3 : MonoBehaviour
{
    private Transform player;
    private Camera cam;

    [Header("Follow")]
    public float smoothSpeed = 6f;
    public float xOffset = 4.5f;      // player nằm bên phải
    public float yOffset = 1.5f;
    public bool onlyMoveRight = true;
    private float minX;

    [Header("Zoom (Orthographic)")]
    public DragonGapChase dragon;
    public float minSize = 7f;        // 🔥 Zoom IN tối đa
    public float maxSize = 10f;       // 🔥 Zoom OUT tối đa
    public float zoomSmooth = 3f;

    [Header("Gap Mapping")]
    public float gapNear = 4.5f;
    public float gapFar = 13f;

    void Start()
    {
        cam = GetComponent<Camera>();
        FindPlayer();
        minX = transform.position.x;
    }

    void LateUpdate()
    {
        if (!player) { FindPlayer(); return; }

        // ===== FOLLOW PLAYER =====
        float desiredX = player.position.x + xOffset;
        float desiredY = player.position.y + yOffset;

        if (onlyMoveRight)
            desiredX = Mathf.Max(minX, desiredX);

        Vector3 desiredPos = new Vector3(desiredX, desiredY, transform.position.z);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            smoothSpeed * Time.deltaTime
        );

        if (onlyMoveRight)
            minX = transform.position.x;

        // ===== ZOOM CONTROL =====
        if (cam.orthographic && dragon != null)
        {
            float gap = dragon.CurrentGapX();

            // Clamp gap để tránh lỗi khi quá xa/quá gần
            float clampedGap = Mathf.Clamp(gap, gapNear, gapFar);

            float t = Mathf.InverseLerp(gapNear, gapFar, clampedGap);

            float targetSize = Mathf.Lerp(minSize, maxSize, t);

            // Clamp chắc chắn không vượt 6–10
            targetSize = Mathf.Clamp(targetSize, minSize, maxSize);

            cam.orthographicSize = Mathf.Lerp(
                cam.orthographicSize,
                targetSize,
                zoomSmooth * Time.deltaTime
            );
        }
    }

    void FindPlayer()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
        else
        {
            var p2 = GameObject.Find("Player");
            if (p2) player = p2.transform;
        }
    }
}