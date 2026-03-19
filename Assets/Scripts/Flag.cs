using System.Collections;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public GameObject winUI;
    private bool isTriggered = false;

    private void Start()
    {

        if (winUI != null)
        {
            winUI.SetActive(false);
        }
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (isTriggered) return;

        PlayerController player = collision.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            isTriggered = true;

            if (winUI == null)
            {
                return;
            }

            StartCoroutine(ShowWinUI());
        }
    }

    private IEnumerator ShowWinUI()
    {
        winUI.SetActive(true);

        yield return null; // đợi 1 frame

        Time.timeScale = 0f;
    }
}