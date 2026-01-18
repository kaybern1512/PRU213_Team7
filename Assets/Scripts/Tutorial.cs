using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial Instance;

    public int currentStep = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool IsStep(int step)
    {
        return currentStep == step;
    }

    public void CompleteStep(int step)
    {
        if (currentStep == step)
        {
            currentStep++;
            Debug.Log("Tutorial step completed: " + step);
        }
    }
}
