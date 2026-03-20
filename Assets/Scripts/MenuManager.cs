using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject defaultRabbitPrefab;

    public void ChangeSkin()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void PlayGame()
    {
        if (CharacterSelect.selectedCharacter == null)
        {
            CharacterSelect.selectedCharacter = defaultRabbitPrefab;
        }

        SceneManager.LoadSceneAsync(2);
    }
    public void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false; // Thoát khi đang test trong Unity
        Application.Quit(); // Thoát khi đã xuất bản game (Build)
    }
}
