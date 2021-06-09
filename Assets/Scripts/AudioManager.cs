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

    public void Play()
    {
        _audioSource.Play();
    }

    public void Stop()
    {
        _audioSource.Stop();
    }

    public AudioClip GetAudio(string audioName)
    {
        return audioDictionary[audioName];
    }

    public void PlayAudio(string audioName)
    {
        AudioClip currentClip = audioDictionary[audioName];
        _audioSource.clip = currentClip;
        _audioSource.Play();
    }

    public void FadeIn(float fadeTime)
    {
        StartCoroutine(_audioFader.FadeIn(fadeTime, _audioSource));
    }

    public void FadeOut(float fadeTime)
    {
        StartCoroutine(_audioFader.FadeOut(fadeTime, _audioSource));
    }
}
