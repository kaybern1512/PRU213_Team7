using UnityEngine;

public class Flag : MonoBehaviour
{
    public GameObject winUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Cham: " + collision.name);

        if (collision.CompareTag("Player"))
        {
            Time.timeScale = 0f;
            winUI.SetActive(true);
        }
    }


}
