using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    public RectTransform arrow;
    public Vector2 offset = new Vector2(0, 100f);

    public Transform player;
    public Transform coin;
    public Transform flag;

    Transform currentTarget;
    int lastStep = -1;

    void Update()
    {
        if (Tutorial.Instance == null) return;

        // ❗ Chỉ xử lý khi step ĐỔI
        if (Tutorial.Instance.currentStep != lastStep)
        {
            lastStep = Tutorial.Instance.currentStep;
            OnStepChanged(lastStep);
        }

        // Follow target hiện tại
        if (currentTarget != null)
        {
            Collider2D col = currentTarget.GetComponent<Collider2D>();

            Vector3 worldPos = currentTarget.position;
            if (col != null)
            {
                // Lấy điểm TRÊN ĐẦU object
                worldPos = col.bounds.max;
            }

            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            arrow.position = screenPos + (Vector3)offset;

        }
    }

    void OnStepChanged(int step)
    {
        arrow.gameObject.SetActive(true);

        switch (step)
        {
            case 0:
                tutorialText.text = "Use A / D to move";
                currentTarget = player;
                break;

            case 1:
                tutorialText.text = "Press SPACE to jump";
                currentTarget = player;
                break;

            case 2:
                tutorialText.text = "Collect a coin";
                currentTarget = coin;
                break;

            case 3:
                tutorialText.text = "Go to the flag";
                currentTarget = flag;
                break;

            default:
                arrow.gameObject.SetActive(false);
                tutorialText.text = "";
                currentTarget = null;
                break;
        }
    }
}
