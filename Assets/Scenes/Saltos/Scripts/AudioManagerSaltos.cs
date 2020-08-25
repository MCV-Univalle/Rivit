using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saltos
{
    public class AudioManagerSaltos : MonoBehaviour
    {
        private static AudioManagerSaltos _instance;
        public static AudioManagerSaltos Instance
        {
            get
            {
                //Logic to create the instance
                if (_instance == null)
                {
                    GameObject go = new GameObject("AudioManagerSaltos");
                    go.AddComponent<AudioManagerSaltos>();
                    _instance = go.GetComponent<AudioManagerSaltos>();
                }
                return _instance;
            }
        }
        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private AudioSource _audioSourceLibelula;

        [SerializeField]
        private AudioClip _timerPop;
        [SerializeField]
        private AudioClip _saltoRanaPlayer;
        [SerializeField]
        private AudioClip _vueloLibelulaCom;
        [SerializeField]
        private AudioClip _gameOverSound;
        [SerializeField]
        private AudioClip _gameWinSound;

        void Awake()
        {
            _instance = this;
        }

        public void PlayTimerPop()
        {
            _audioSource.clip = _timerPop;
            _audioSource.Play();
        }

        public void PlaySaltoRanaPlayer()
        {
            _audioSource.clip = _saltoRanaPlayer;
            _audioSource.Play();
        }
        public void PlayVueloLibelulaCom()
        {
            _audioSourceLibelula.clip = _vueloLibelulaCom;
            _audioSourceLibelula.Play();
        }

        public void PlayGameOver()
        {
            _audioSource.clip = _gameOverSound;
            _audioSource.Play();
        }

        public void PlayWinGame()
        {
            _audioSource.clip = _gameWinSound;
            _audioSource.Play();
        }

    }
}