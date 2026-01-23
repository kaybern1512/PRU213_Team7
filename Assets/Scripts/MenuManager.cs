using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject defaultRabbitPrefab;

    public void ChangeSkin()
    {
        SceneManager.LoadScene("ChangeCharacter");
    }

    public void PlayGame()
    {
        if (CharacterSelect.selectedCharacter == null)
        {
            CharacterSelect.selectedCharacter = defaultRabbitPrefab;
        }

        SceneManager.LoadScene("GameScene2");
    }
}
