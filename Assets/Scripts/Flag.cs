using UnityEngine;

public class Flag : MonoBehaviour
{
    public GameObject winUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // ===== TUTORIAL STEP 3 =====
        if (Tutorial.Instance != null && Tutorial.Instance.IsStep(3))
        {
            Tutorial.Instance.CompleteStep(3);
        }

        Time.timeScale = 0f;
        winUI.SetActive(true);
    }

}
