using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroMelodia
{
    public class Frog : MonoBehaviour
    {
        public delegate void SingDelegate(int note);
        public static event SingDelegate singNote;
        public static event SingDelegate stopSinging;

        [SerializeField] private int noteNumber;
        [SerializeField] private Attention attention;
        [SerializeField] private GameObject frogReflect;
        private AudioSource _audioSource;
        private AudioFader _audioFader;
        private IEnumerator fadingCoroutine;
        private FrogAnimation _frogAnimation;
        private bool _isClickAvaible = true;
        private bool _isPaused = false;
        public bool Enable { get; set; }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _frogAnimation = new FrogAnimation(gameObject, frogReflect);
            _audioFader = new AudioFader();
        }

        void Start()
        {
            GameManager.endGame += CloseMouth;
            GameManager.endGame += TriggerOffSurprise;
            UIManager.executePauseButton += PauseSing;
            UIManager.executeResumeFromPause += PauseSing;
            Enable = true;
        }
        private void OnDisable()
        {
            GameManager.endGame -= CloseMouth;
            GameManager.endGame -= TriggerOffSurprise;
            UIManager.executePauseButton -= PauseSing;
            UIManager.executeResumeFromPause -= PauseSing;
            StopAllCoroutines();
        }
        void OnMouseDown()
        {
            if (_isClickAvaible && Enable)
            {
                _isClickAvaible = !_isClickAvaible;
                OpenMouth();
                singNote(noteNumber);
            }
        }
        void OnMouseUp()
        {
            if (Enable)
            {
                _isClickAvaible = !_isClickAvaible;
                CloseMouth();
                stopSinging(noteNumber);
            }
        }

        public void OpenMouth()
        {
            if (fadingCoroutine != null)
                StopCoroutine(fadingCoroutine);
            _audioSource.volume = 1;
            _frogAnimation.OpenFrog(true);
            _audioSource.Play();
        }

        public void CloseMouth()
        {
            _frogAnimation.OpenFrog(false);
            fadingCoroutine = _audioFader.FadeOut(0.1F, _audioSource);
            StartCoroutine(fadingCoroutine);
        }

        void OnEnable()
        {
            _frogAnimation.AppearFrog(true);
            StartCoroutine(Blink());
            StartCoroutine(Breath());
        }

        public IEnumerator Sing(float speed)
        {
            OpenMouth();
            yield return new WaitForSeconds(speed);
            CloseMouth();
        }

        public void PauseSing()
        {
            _isPaused = !_isPaused;
            if (_isPaused)
                _audioSource.Pause();
            else
                _audioSource.UnPause();
        }

        public IEnumerator Blink()
        {
            while (gameObject.activeInHierarchy)
            {
                int randomNum = Random.Range(0, 15);
                if (randomNum == 5)
                    _frogAnimation.BlinkFrog();
                yield return new WaitForSeconds(0.5F);
            }
        }

        public IEnumerator Breath()
        {
            while (gameObject.activeInHierarchy)
            {
                int randomNum = Random.Range(0, 15);
                if (randomNum == 5)
                    _frogAnimation.BreathFrog();
                yield return new WaitForSeconds(0.75F);
            }
        }

        public void TriggerSurprise()
        {
            _frogAnimation.SurpriseFrog(true);
            attention.Activate(this.gameObject);
        }
        public void TriggerOffSurprise()
        {
            _frogAnimation.SurpriseFrog(false);
            attention.Desactivate();
        }
    }
}
