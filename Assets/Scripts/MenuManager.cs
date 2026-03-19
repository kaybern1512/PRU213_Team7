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
}
