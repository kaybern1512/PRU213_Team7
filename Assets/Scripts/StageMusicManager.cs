using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMusicManager : MonoBehaviour
{
    public static StageMusicManager instance;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip stageMusic;
    public AudioClip bossMusic;
    public float fadeDuration = 1.0f;

    private bool isBossMusicPlaying = false;
    private float targetVolume;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        targetVolume = audioSource.volume;
        if (targetVolume <= 0) targetVolume = 1f;

        // Bắt đầu với nhạc Stage
        audioSource.clip = stageMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlayBossMusic()
    {
        if (!isBossMusicPlaying)
        {
            isBossMusicPlaying = true;
            StartCoroutine(FadeToTrack(bossMusic));
        }
    }

    private IEnumerator FadeToTrack(AudioClip newClip)
    {
        // Fade Out nhạc cũ
        float startVol = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVol * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.volume = 0;
        audioSource.Play();

        // Fade In nhạc Boss
        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += targetVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
        audioSource.volume = targetVolume;
    }
}