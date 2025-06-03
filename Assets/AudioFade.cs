using UnityEngine;
using System.Collections;

public class AudioFade : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeInDuration;
    public float fadeOutDuration;
    [SerializeField] float StartVolume;
    [SerializeField] float TargetVolume;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Ensure the audio starts muted if you want to fade in
        audioSource.volume = 0;
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {

        audioSource.volume = StartVolume;
        audioSource.Play();

        while (audioSource.volume < TargetVolume)
        {
            audioSource.volume += Time.deltaTime / fadeInDuration;
            yield return null;
        }

        audioSource.volume = TargetVolume;
    }

    private IEnumerator FadeOutCoroutine()
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeOutDuration;
            yield return null;
        }

        audioSource.Stop();
    }
}
