using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSource;
    private AudioFader _audioFader;
    [SerializeField] private StringAudioDictionary audioDictionary;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioFader = new AudioFader();
    }

    public void PlayAudio(string audioName)
    {
        AudioClip currentClip = audioDictionary[audioName];
        _audioSource.clip = currentClip;
        _audioSource.Play();
    }

    public void FadeOut(float fadeTime)
    {
        _audioFader.FadeOut(fadeTime, _audioSource);
    }
}
