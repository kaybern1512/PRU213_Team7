using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip jumpSound;
    public AudioClip itemSound;
    public AudioClip hurtSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayJump()
    {
        audioSource.PlayOneShot(jumpSound);
    }

    public void PlayItem()
    {
        audioSource.PlayOneShot(itemSound);
    }

    public void PlayHurt()
    {
        audioSource.PlayOneShot(hurtSound);
    }
}