using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("GameOverUI Awake");
        gameObject.SetActive(false);
    }

    public void Show()
    {
        Debug.Log("GameOverUI Show CALLED");
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
}
