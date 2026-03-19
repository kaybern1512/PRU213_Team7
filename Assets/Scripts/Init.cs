using UnityEngine;
using UnityEngine.UI;

public class Init : MonoBehaviour
{
    public Image healthBar;
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

        PlayerController pc = player.GetComponent<PlayerController>();

        if (pc != null)
        {
            pc.healthBar = healthBar;
            healthBar.fillAmount = (float)pc.health / pc.maxHealth;
        }
    }
}
