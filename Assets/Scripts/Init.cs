using UnityEngine;

public class Init : MonoBehaviour
{
    void Start()
    {
        GameObject selectedCharacter = CharacterSelect.selectedCharacter;

        Debug.Log("Init.Start() is running");

        if (selectedCharacter == null)
        {
            Debug.LogWarning("selectedCharacter is NULL");
            return;
        }

        Debug.Log("Selected Character = " + selectedCharacter.name);

        GameObject player = Instantiate(
            selectedCharacter,
            transform.position,
            Quaternion.identity
        );

        player.name = "Player";
    }
}
