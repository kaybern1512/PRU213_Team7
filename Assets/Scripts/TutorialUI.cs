using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;

    void Update()
    {
        Debug.Log("TutorialUI is running");
        if (Tutorial.Instance == null) return;

        switch (Tutorial.Instance.currentStep)
        {
            case 0:
                tutorialText.text = "Use A / D to move";
                break;

            case 1:
                tutorialText.text = "Press SPACE to jump";
                break;

            case 2:
                tutorialText.text = "Collect a coin";
                break;

            case 3:
                tutorialText.text = "Go to the flag";
                break;

            default:
                gameObject.SetActive(false); // xong tutorial
                break;
        }
    }
}
