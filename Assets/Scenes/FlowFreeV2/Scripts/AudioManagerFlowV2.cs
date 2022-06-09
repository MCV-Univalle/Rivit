using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFreeV2
{
    public class AudioManagerFlowV2 : MonoBehaviour
    {
        public static AudioManagerFlowV2 _instance;
        private AudioSource _audioSource;
        [SerializeField] private StringAudioDictionary audioDictionary;
        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
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
    }
}