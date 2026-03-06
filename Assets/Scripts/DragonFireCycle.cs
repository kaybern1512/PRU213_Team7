using System.Collections;
using UnityEngine;

public class DragonFireCycle : MonoBehaviour
{
    [Header("Refs")]
    public GameObject fireVFXRoot;
    public SpriteRenderer fireRenderer;
    public BoxCollider2D hitbox;

    [Header("Timing")]
    public float startDelay = 5f;
    public float fireInterval = 8f;

    [Tooltip("Thời gian lửa hiện ra trước khi bắt đầu gây damage")]
    public float preFireShowTime = 0.2f;

    [Tooltip("Tổng thời gian lửa hiển thị")]
    public float fireVisualDuration = 1.0f;

    [Tooltip("Tổng thời gian gây damage")]
    public float fireDamageDuration = 0.65f;

    [Header("Hitbox Grow (left -> right)")]
    public float growTime = 0.2f;
    public Vector2 startSize = new Vector2(0.25f, 0.9f);
    public Vector2 endSize = new Vector2(2.4f, 0.9f);

    public float offsetY = 0f;

    [Tooltip("Offset hitbox theo chiều dài. Giảm số này để bớt vươn xa.")]
    [Range(0.2f, 0.5f)] public float offsetFactor = 0.4f;

    [Header("Damage / Knockback")]
    public int damage = 20;
    public float damageCooldown = 1f;
    public float knockbackRight = 6f;
    public float knockbackUp = 3f;

    [Header("Optional: trigger anim on DragonHead")]
    public Animator dragonAnimator;
    public string fireTriggerName = "Fire";

    public bool IsFiring => firing;
    private bool firing;

    private float lastDamageTime = -999f;

    void Start()
    {
        if (fireVFXRoot != null)
            fireVFXRoot.SetActive(true);

        if (fireRenderer != null)
            fireRenderer.enabled = false;

        if (hitbox != null)
        {
            hitbox.enabled = false;
            ApplyHitbox(startSize);
        }

        StartCoroutine(FireLoop());
    }

    void Update()
    {
        if (!firing || hitbox == null || !hitbox.enabled)
            return;

        Collider2D player = Physics2D.OverlapBox(
            (Vector2)hitbox.transform.position + hitbox.offset,
            hitbox.size,
            0f
        );

        if (player != null && player.CompareTag("Player"))
        {
            TryDamagePlayer(player);
        }
    }

    IEnumerator FireLoop()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            yield return FireOnce();
            yield return new WaitForSeconds(fireInterval);
        }
    }

    IEnumerator FireOnce()
    {
        if (firing) yield break;
        firing = true;

        if (dragonAnimator != null && !string.IsNullOrEmpty(fireTriggerName))
        {
            dragonAnimator.ResetTrigger(fireTriggerName);
            dragonAnimator.SetTrigger(fireTriggerName);
        }

        if (fireRenderer != null)
            fireRenderer.enabled = true;

        if (hitbox != null)
        {
            ApplyHitbox(startSize);
            hitbox.enabled = false;
        }

        yield return new WaitForSeconds(preFireShowTime);

        if (hitbox != null)
            hitbox.enabled = true;

        float t = 0f;
        float gt = Mathf.Max(0.01f, growTime);

        while (t < gt)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / gt);

            if (hitbox != null)
            {
                Vector2 size = Vector2.Lerp(startSize, endSize, k);
                ApplyHitbox(size);
            }

            yield return null;
        }

        float remainDamage = Mathf.Max(0f, fireDamageDuration - growTime);
        if (remainDamage > 0f)
            yield return new WaitForSeconds(remainDamage);

        if (hitbox != null)
            hitbox.enabled = false;

        float remainVisual = Mathf.Max(0f, fireVisualDuration - preFireShowTime - fireDamageDuration);
        if (remainVisual > 0f)
            yield return new WaitForSeconds(remainVisual);

        if (fireRenderer != null)
            fireRenderer.enabled = false;

        firing = false;
    }

    void ApplyHitbox(Vector2 size)
    {
        if (hitbox == null) return;

        hitbox.size = size;
        hitbox.offset = new Vector2(size.x * offsetFactor, offsetY);
    }

    void TryDamagePlayer(Collider2D playerCol)
    {
        if (Time.time < lastDamageTime + damageCooldown)
            return;

        lastDamageTime = Time.time;

        // Gọi hàm damage có sẵn bên Player
        playerCol.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

        // Knockback: đẩy sang phải + hất lên, nhưng không phá state nhảy quá mạnh
        Rigidbody2D rb = playerCol.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 v = rb.linearVelocity;
            v.x = knockbackRight;
            v.y = Mathf.Max(v.y, knockbackUp);
            rb.linearVelocity = v;

           
        }
    }

    void OnDrawGizmosSelected()
    {
        if (hitbox == null) return;

        Gizmos.color = Color.red;
        Vector2 center = (Vector2)hitbox.transform.position + hitbox.offset;
        Gizmos.DrawWireCube(center, hitbox.size);
    }
}