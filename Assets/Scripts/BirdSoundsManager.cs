using UnityEngine;
using System.Collections;

public class BirdSoundsManager : MonoBehaviour
{
    public AudioSource[] BirdSounds;
    public float MinInterval;
    public float MaxInterval;
    public float MinChirpingInterval;
    public float MaxChirpingInterval;

    private int _currentIndex;

    void Start()
    {
        StartCoroutine(PlayRandomBirdSounds());
    }

    IEnumerator PlayRandomBirdSounds()
    {
        while (true)
        {

            // Wait for a random interval
            yield return new WaitForSeconds(Random.Range(MinInterval, MaxInterval));
            // Choose a random bird sound
            _currentIndex = Random.Range(0, BirdSounds.Length);

            if (Random.Range(0, 1)==0)
            {

                float chirpingDelay = Random.Range(MinChirpingInterval, MaxChirpingInterval);
                BirdSounds[_currentIndex].Play();
                yield return new WaitForSeconds(chirpingDelay);
                while (Random.Range(0, 3) < 2)
                {
                    BirdSounds[_currentIndex].Play();
                    yield return new WaitForSeconds(chirpingDelay);
                }
            }
                BirdSounds[_currentIndex].Play();
        }
    }
}
