using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFader
{
    public IEnumerator FadeOut(float fadeTime, AudioSource audioSource)
    {
        float initialVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= initialVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = 1;
    }

    public IEnumerator FadeIn(float fadeTime, AudioSource audioSource)
    {
        float initialVolume = audioSource.volume;
        audioSource.volume = 0;
        while (audioSource.volume < 1)
        {
            audioSource.volume += initialVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
       
    }
}
