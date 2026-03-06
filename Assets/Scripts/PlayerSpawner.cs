using UnityEngine;

public class PlayerSpawner : MonoBehaviour { void Start() { if (CharacterSelect.selectedCharacter == null) { Debug.LogWarning("selectedCharacter is null! Chưa chọn nhân vật hoặc vào GameScene trực tiếp."); return; } Instantiate(CharacterSelect.selectedCharacter, transform.position, Quaternion.identity); } }