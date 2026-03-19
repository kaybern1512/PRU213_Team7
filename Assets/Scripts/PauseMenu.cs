using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseOverlay;
    public TMP_Text soundButtonText;
    public Slider volumeSlider;
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;
    private bool isMuted = false;

    private void Start()
    {
        Time.timeScale = 1f;

        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
        float savedVolume = PlayerPrefs.GetFloat("GameVolume", 1f);

        if (volumeSlider != null)
            volumeSlider.value = savedVolume;

        AudioListener.volume = isMuted ? 0f : savedVolume;

        UpdateSoundButtonText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if (pauseOverlay != null)
            pauseOverlay.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ToggleSound()
    {
        isMuted = !isMuted;

        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
        PlayerPrefs.Save();

        float currentVolume = PlayerPrefs.GetFloat("GameVolume", 1f);
        AudioListener.volume = isMuted ? 0f : currentVolume;

        UpdateSoundButtonText();
    }

    public void OnVolumeChanged(float value)
    {
        Debug.Log("Volume changed: " + value);
        PlayerPrefs.SetFloat("GameVolume", value);
        PlayerPrefs.Save();

        if (!isMuted)
            AudioListener.volume = value;
    }

    private void UpdateSoundButtonText()
    {
        if (soundButtonText != null)
            soundButtonText.text = isMuted ? "SOUND: OFF" : "SOUND: ON";
    }
}