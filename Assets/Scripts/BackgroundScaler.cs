using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        // Reset scale về 1
        transform.localScale = Vector3.one;

        float width = sr.sprite.bounds.size.x;
        float height = sr.sprite.bounds.size.y;

        // Tính toán kích thước màn hình trong không gian thế giới
        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        // Ép scale của ảnh khớp với kích thước màn hình
        transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 1);
    }
}