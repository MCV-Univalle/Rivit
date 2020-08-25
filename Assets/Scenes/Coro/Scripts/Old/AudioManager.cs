using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
namespace CoroMelodia
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
        private AudioClip[] _frogsChantVoicesList;
        [SerializeField]
        private GameObject _chantPrefab;
        private ChantScript _currentChant;

        [SerializeField]
        private AudioClip _metronomeClack;
        [SerializeField]
        private AudioClip _bounce;
        [SerializeField]
        private AudioClip _correct;
        [SerializeField]
        private AudioClip _wrong;
        [SerializeField]
        private AudioClip _lightOff;
        [SerializeField]
        private AudioClip _lightOn;

        void Awake()
        {
            _instance = this;
            //Get the audio clip for every frog
            //Assumptions: there are only 6 frogs
            _frogsChantVoicesList = new AudioClip[6];
            GameObject[] frogList = FrogsManager.Instance.FrogList;
            for(int i = 0; i < 6; i++)
            {
                _frogsChantVoicesList[i] = frogList[i].GetComponent<FrogsClickEvent>().ChantVoice; 
            }  
        }

        public void PlayChant(int numFrog)
        {
            AudioClip chantVoice = _frogsChantVoicesList[numFrog];
            _currentChant = Instantiate(_chantPrefab).GetComponent<ChantScript>();
            _currentChant.PlayChant(chantVoice);
        }
        public void PauseChant()
        {
            if(_currentChant != null) _currentChant.PauseChant();
        }
        public void FadeOut(float fadeTime)
        {
            StartCoroutine(_currentChant.FadeOut(fadeTime));
        }

        public void PlayBounce()
        {
            _audioSource.clip = _bounce;
            _audioSource.Play();
        }

        public void PlayMetronomeClack()
        {
            _audioSource.clip = _metronomeClack;
            _audioSource.Play();
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
        public void PlayLightOff()
        {
            _audioSource.clip = _lightOff;
            _audioSource.Play();
        }
        public void PlayLightOn()
        {
            _audioSource.clip = _lightOn;
            _audioSource.Play();
        }
    }
}
*/