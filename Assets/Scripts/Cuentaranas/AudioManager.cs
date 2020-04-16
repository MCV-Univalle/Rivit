using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cuentaranas
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;
        public static AudioManager Instance
        {
            get
            {
                //Logic to create the instance
                if(_instance == null)
                {
                    GameObject go = new GameObject("AudioManager");
                    go.AddComponent<AudioManager>();
                    _instance = go.GetComponent<AudioManager>(); 
                }
                return _instance;
            }
        }

        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private AudioClip _correct;
        [SerializeField]
        private AudioClip _wrong;

        void Awake()
        {
            _instance = this;
            _audioSource = this.GetComponent<AudioSource>();
        }

        public void PlayCorrect()
        {
            _audioSource.clip = _correct;
            _audioSource.Play();
        }
        public void PlayWrong()
        {
            _audioSource.clip = _wrong;
            _audioSource.Play();
        }

    }
}

