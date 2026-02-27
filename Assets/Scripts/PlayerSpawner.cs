using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawner : MonoBehaviour
{
    public Image healthImage;   // kéo Health UI vào đây

    void Start()
    {
        if (CharacterSelect.selectedCharacter == null)
        {
            Debug.LogWarning("selectedCharacter is null! Chưa chọn nhân vật hoặc vào GameScene trực tiếp.");
            return;
        }

        GameObject player = Instantiate(
            CharacterSelect.selectedCharacter,
            transform.position,
            Quaternion.identity
        );

        // GÁN UI CHO PLAYER
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.healthImage = healthImage;
        }
        else
        {
            Debug.LogWarning("PlayerController not found on spawned player!");
        }
    }
}
