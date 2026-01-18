using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;   // 🔴 QUAN TRỌNG

public class PauseMenu : MonoBehaviour
{
    public GameObject container;

    void Start()
    {
        if (container != null)
            container.SetActive(false);

        Time.timeScale = 1f;
    }

    void Update()
    {
        // ===== INPUT SYSTEM (NEW) =====
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            container.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    // ===== NÚT CONTINUE =====
    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1f;
    }

    // ===== NÚT MAIN MENU =====
    public void MainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // đổi tên scene nếu cần
    }
}
