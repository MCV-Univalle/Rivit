using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudio : MonoBehaviour
{
    private static UIAudio _instance;
    public static UIAudio Instance
    {
        get
        {
            //Logic to create the instance
            if(_instance == null)
            {
                GameObject go = new GameObject("UIAudio");
                go.AddComponent<UIAudio>();
                _instance = go.GetComponent<UIAudio>();
            }
            return _instance;
        }
    }
        
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _confirmationClip;
    [SerializeField]
    private AudioClip _cancelClip;
    [SerializeField]
    private AudioClip _pauseClip;
    [SerializeField]
    private AudioClip _carouselTransitionClip;
    [SerializeField]
    private AudioClip _countingClip;

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("UIAudio");
        if(objs.Length > 1) Destroy(this.gameObject);
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            _audioSource = this.GetComponent<AudioSource>();
        }
    }

    public void PlayConfirmationClip()
    {
            _audioSource.clip = _confirmationClip;
            _audioSource.Play();
    }
    public void PlayCancelClip()
    {
            _audioSource.clip = _cancelClip;
            _audioSource.Play();
    }
    public void PlayPauseClip()
    {
            _audioSource.clip = _pauseClip;
            _audioSource.Play();
    }
    public void PlayCarouselTransitionClip()
    {
            _audioSource.clip = _carouselTransitionClip;
            _audioSource.Play();
    }
    public void PlayCountingClip()
    {
            _audioSource.clip = _countingClip;
            _audioSource.Play();
    }
}
