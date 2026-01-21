using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
   public void LoadCurrentScene()
    {
        SceneManager.LoadSceneAsync(1);
        Time.timeScale = 1f;
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
        Time.timeScale = 1f;
    }
}
