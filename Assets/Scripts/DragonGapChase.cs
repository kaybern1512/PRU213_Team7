using System.Collections;
using UnityEngine;

public class DragonGapChase : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D playerRb;
    private Animator dragonAnimator;

    [Header("Camera")]
    public Camera cam;

    [Header("Start")]
    public float startDelay = 2f;
    public bool snapVirtualOnStart = true;
    private float chaseStartTime;
    private bool chaseEnabled;

    [Header("Gap (world units) - tuned for player speed ~ 8")]
    public float desiredGap = 9.0f;
    public float minGap = 6.5f;
    public float maxGap = 16f;

    [Header("Speed (logic chase) - slower")]
    public float baseSpeed = 2.8f;
    public float catchUpSpeed = 5.0f;
    public float retreatSpeed = 1.6f;
    public float accel = 6f;
    public float errorGain = 0.22f;

    [Header("Screen Anchor (LEFT) - render only")]
    [Range(0f, 0.2f)] public float xFar = 0.03f;
    [Range(0f, 0.4f)] public float xNear = 0.15f;
    [Range(0f, 0.6f)] public float xBiteLunge = 0.26f; // ✅ lao vô: thò sâu hơn xNear
    [Range(0f, 1f)] public float viewportY = 0.55f;

    [Header("Pressure smoothing")]
    public float pressureSmooth = 5f;

    [Header("Vertical feel (NOT follow jump 1:1)")]
    public float baseY = 0f;
    public float yOffset = 0.6f;
    public float yFollowRange = 1.2f;
    public float ySmooth = 0.25f;

    // ===== Bite / Lunge =====
    [Header("Bite (lunge + knockback)")]
    public bool enableBite = true;

    public float biteTriggerGap = 6.8f;   // ✅ gap <= cái này thì có thể lao cắn
    public float biteCooldown = 1.2f;     // ✅ thời gian hồi giữa các lần cắn
    public float biteLungeTime = 0.25f;   // ✅ thời gian thò vào sâu

    public float knockbackX = 6.0f;       // ✅ đẩy player sang phải
    public float knockbackY = 1.5f;       // ✅ nhấc nhẹ (0 nếu không muốn)

    public string biteTriggerName = "Bite"; // ✅ Trigger trong Animator (đổi nếu bạn đặt khác)

    private float nextBiteTime = 0f;
    private bool isLunging = false;

    // runtime
    private float currentSpeed;
    private float yVel;

    private float virtualX;
    [Range(0f, 1f)] public float pressure;
    private float currentViewportX;

    void Start()
    {
        if (cam == null) cam = Camera.main;
        currentViewportX = xFar;
        chaseEnabled = false;

        // Optional: nếu có Animator trên DragonHead thì tự lấy
        dragonAnimator = GetComponent<Animator>();
    }

    void FindPlayerIfNeeded()
    {
        if (player != null) return;

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        if (player == null)
        {
            var p2 = GameObject.Find("Player");
            if (p2 != null) player = p2.transform;
        }

        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();

            chaseStartTime = Time.time;
            chaseEnabled = false;

            virtualX = player.position.x - desiredGap;
            currentSpeed = 0f;

            if (Mathf.Approximately(baseY, 0f))
                baseY = player.position.y;
        }
    }

    void LateUpdate()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        FindPlayerIfNeeded();
        if (player == null) return;

        if (!chaseEnabled)
        {
            if (Time.time - chaseStartTime < startDelay)
            {
                RenderFromCameraAnchor();
                return;
            }

            chaseEnabled = true;

            if (snapVirtualOnStart)
            {
                virtualX = player.position.x - desiredGap;
                currentSpeed = 0f;
            }
        }

        // ===== LOGIC CHASE (X only) =====
        float dx = player.position.x - virtualX;

        float targetSpeed;
        if (dx > maxGap)
        {
            targetSpeed = catchUpSpeed;
        }
        else if (dx < minGap)
        {
            targetSpeed = -retreatSpeed;
        }
        else
        {
            float error = dx - desiredGap;
            targetSpeed = baseSpeed + error * errorGain;
            targetSpeed = Mathf.Clamp(targetSpeed, 1.2f, catchUpSpeed);
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.deltaTime);
        virtualX += currentSpeed * Time.deltaTime;

        // ===== PRESSURE from gap =====
        float t = Mathf.InverseLerp(minGap, maxGap, dx);
        float targetPressure = 1f - t;
        pressure = Mathf.Lerp(pressure, targetPressure, pressureSmooth * Time.deltaTime);

        // ===== BITE CHECK (lao vô + đẩy nhẹ) =====
        // chỉ cắn khi: bật enable, chưa đang lunge, hết cooldown, và gap đủ gần
        if (enableBite && !isLunging && Time.time >= nextBiteTime && dx <= biteTriggerGap)
        {
            nextBiteTime = Time.time + biteCooldown;
            StartCoroutine(BiteLunge());
        }

        RenderFromCameraAnchor();
    }

    IEnumerator BiteLunge()
    {
        isLunging = true;

        // Trigger animation nếu có
        if (dragonAnimator != null && !string.IsNullOrEmpty(biteTriggerName))
        {
            dragonAnimator.ResetTrigger(biteTriggerName);
            dragonAnimator.SetTrigger(biteTriggerName);
        }

        // giữ trạng thái lunge trong 1 khoảng thời gian ngắn
        yield return new WaitForSeconds(biteLungeTime);

        // knockback player (đẩy nhẹ) — không gây damage ở đây
        if (playerRb != null)
        {
            Vector2 v = playerRb.linearVelocity;
            v.x = Mathf.Max(v.x, knockbackX);
            v.y = Mathf.Max(v.y, knockbackY);
            playerRb.linearVelocity = v;
        }

        isLunging = false;
    }

    void RenderFromCameraAnchor()
    {
        // Nếu đang lunge thì thò sâu hơn xNear
        float nearX = isLunging ? xBiteLunge : xNear;

        float targetVX = Mathf.Lerp(xFar, nearX, pressure);
        currentViewportX = Mathf.Lerp(currentViewportX, targetVX, pressureSmooth * Time.deltaTime);

        Vector3 anchor = cam.ViewportToWorldPoint(
            new Vector3(currentViewportX, viewportY, -cam.transform.position.z)
        );

        float playerY = player.position.y;
        float wantedY = Mathf.Clamp(playerY, baseY - yFollowRange, baseY + yFollowRange) + yOffset;

        float newY = Mathf.SmoothDamp(transform.position.y, wantedY, ref yVel, ySmooth);

        transform.position = new Vector3(anchor.x, newY, transform.position.z);
    }

    public float CurrentGapX()
    {
        if (!player) return desiredGap;
        return player.position.x - virtualX;
    }
}