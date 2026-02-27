using UnityEngine;
using UnityEngine.UI;

public class Init : MonoBehaviour
{
    public Image healthImage; // KÉO FILL IMAGE VÀO ĐÂY

    void Start()
    {
        GameObject selectedCharacter = CharacterSelect.selectedCharacter;

        if (selectedCharacter == null)
        {
            Debug.LogWarning("selectedCharacter is NULL");
            return;
        }

        GameObject player = Instantiate(
            selectedCharacter,
            transform.position,
            Quaternion.identity
        );

        player.name = "Player";

        // ===== GÁN UI CHO PLAYER =====
        PlayerController pc = player.GetComponent<PlayerController>();

        if (pc == null)
        {
            Debug.LogError("PlayerController NOT FOUND on player prefab");
            return;
        }

        pc.healthImage = healthImage;
        pc.UpdateHealthUI(); // gọi cập nhật ngay
    }
}
