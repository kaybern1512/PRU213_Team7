using System.Collections;
using UnityEngine;

public class DragonGapChase : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D playerRb;
    private PlayerController playerPc;

    [Header("Start")]
    public float startDelay = 2f;
    public bool snapVirtualOnStart = true;
    private float chaseStartTime;
    private bool chaseEnabled;

    [Header("Gap")]
    public float desiredGap = 10.5f;
    public float minGap = 7.5f;
    public float maxGap = 15f;

    [Header("Speed")]
    public float baseSpeed = 4.5f;
    public float catchUpSpeed = 8.5f;
    public float retreatSpeed = 2.5f;
    public float accel = 10f;
    public float errorGain = 0.18f;

    [Header("Vertical follow")]
    public float baseY = 0f;
    public float yOffset = 0.8f;
    public float yFollowRange = 1.26f;
    public float ySmooth = 0.12f;

    [Header("Fire Lock")]
    public DragonFireCycle fireCycle;
    public float noBiteAfterFire = 0.4f;
    private float noBiteUntilTime = 0f;

    [Header("Near Damage")]
    public bool enableNearDamage = true;
    public float biteTriggerGap = 6.8f;
    public float biteCooldown = 1.2f;
    public int biteDamage = 20;

    [Header("Knockback")]
    public float knockbackX = 6f;
    public float knockbackY = 1.5f;

    private float nextBiteTime = 0f;

    // runtime
    private float currentSpeed;
    private float yVel;
    private float virtualX;

    void FindPlayerIfNeeded()
    {
        if (player != null) return;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p == null) p = GameObject.Find("Player");

        if (p != null)
        {
            player = p.transform;
            playerRb = p.GetComponent<Rigidbody2D>();
            playerPc = p.GetComponent<PlayerController>();

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
        FindPlayerIfNeeded();
        if (player == null) return;

        if (!chaseEnabled)
        {
            if (Time.time - chaseStartTime < startDelay)
            {
                RenderDragon();
                return;
            }

            chaseEnabled = true;

            if (snapVirtualOnStart)
            {
                virtualX = player.position.x - desiredGap;
                currentSpeed = 0f;
            }
        }

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

        bool firingNow = (fireCycle != null && fireCycle.IsFiring);
        if (firingNow)
            noBiteUntilTime = Time.time + noBiteAfterFire;

        bool canDamageNear =
            enableNearDamage &&
            Time.time >= nextBiteTime &&
            Time.time >= noBiteUntilTime &&
            !firingNow &&
            dx <= biteTriggerGap;

        if (canDamageNear)
        {
            DoNearDamage();
            nextBiteTime = Time.time + biteCooldown;
        }

        RenderDragon();
    }

    void DoNearDamage()
    {
        if (playerPc != null)
        {
            playerPc.health -= biteDamage;
            Debug.Log("[Dragon Near Hit] Player HP: " + playerPc.health);

            if (playerPc.health <= 0)
            {
                playerPc.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
            }
        }

        if (playerRb != null)
        {
            Vector2 v = playerRb.linearVelocity;
            v.x = Mathf.Max(v.x, knockbackX);
            v.y = Mathf.Max(v.y, knockbackY);
            playerRb.linearVelocity = v;
        }
    }

    void RenderDragon()
    {
        if (player == null) return;

        float playerY = player.position.y;
        float wantedY = Mathf.Clamp(playerY, baseY - yFollowRange, baseY + yFollowRange) + yOffset;
        float newY = Mathf.SmoothDamp(transform.position.y, wantedY, ref yVel, ySmooth);

        transform.position = new Vector3(virtualX, newY, transform.position.z);
    }

    public float CurrentGapX()
    {
        if (!player) return desiredGap;
        return player.position.x - virtualX;
    }
}