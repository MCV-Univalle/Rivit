using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
namespace CoroMelodia
{
    public class UIManager : UIController
    {
        private static UIManager _instance;
        public static UIManager Instance
        {
            get
            {
                //Logic to create the instance
                if(_instance == null)
                {
                    GameObject go = new GameObject("UIManager");
                    go.AddComponent<UIManager>();
                    _instance = go.GetComponent<UIManager>();
                }
                return _instance;
            }
        }
    
        [SerializeField]
        private TextMeshProUGUI _numberMelodyNotes;
        public bool GaugeIsActive {get; set;}
        public bool MainScreenIsActive {get; set;}
        private float _tempoAmount = 0.3F;
        [SerializeField]
        private GameObject _tempoGauge;
        [SerializeField]
        private GameObject _gaugeFillCircle;
        [SerializeField]
        private GameObject _gaugeMusicIcon;

        private float _initialTime = 0.0F;
        private float _inactiveDuration = 1F;

        [SerializeField]
        private Color _redColor;
        [SerializeField]
        private Color _blueColor;
        [SerializeField]
        private Color _yellowColor;
        [SerializeField]
        private Color _purpleColor;
        [SerializeField]
        private Color _brownColor;
        [SerializeField]
        private Color _blackColor;
        
        void Awake()
        {
            _instance = this;
            GaugeIsActive = false;
            MainScreenIsActive = false;
        }

        void Start()
        {
            _gameManager = GameManager.Instance;
            base.Start();
            StartCoroutine(ShowMainScreen(1.3F));
        }

        void Update()
        {
            IncrementGauge();
            CheckInactiveTime();
            base.Update();
        }

        public IEnumerator UpdateNumberMelodyNotes()
        {
            int currentDisplayNumber =  int.Parse(_numberMelodyNotes.text);
            int num = MelodyManager.Instance.MelodyNotes.Count;
            for(int i = currentDisplayNumber; i <= num; i++)
            {
                _numberMelodyNotes.text = "" + i;
                yield return new WaitForSeconds(0.25F);
            }
        }

        public void ExecuteHelpButton()
        {
            base.ExecuteHelpButton();
            FrogsManager.Instance.AreFrogsBlocked = true;
            StartCoroutine(FadeElement(MessagesManager.Instance.PressFrogIndication, false));
            MainScreenIsActive = false;
        }

        public void ExecuteCloseButton()
        {
            base.ExecuteCloseButton();
            AnimationController.Instance.ToDirectorFailPose(false);
            AnimationController.Instance.ToDirectorCorrectPose(false);
            FrogsManager.Instance.AreFrogsBlocked = false;
            MainScreenIsActive = true;
            MessagesManager.Instance.PressFrogIndication.SetActive(false);
            StartCoroutine(ShowPressFrogIndication(1F));
        }

        public void ExecutePlayButton()
        {
            base.ExecutePlayButton();
            FrogsManager.Instance.AreFrogsBlocked = true;
            MainScreenIsActive = false;
            StartCoroutine(FadeElement(MessagesManager.Instance.PressFrogIndication, false));
        }

        public void ExecutePauseButton()
        {
            base.ExecutePauseButton();
            //GameManager.Instance.IsPaused = true;
            StartCoroutine(GameManager.Instance.PauseChant());
        }

        public void ExecuteResumeButton()
        {
            //GameManager.Instance.IsPaused = false;
            base.ExecuteResumeButton();
        }

        public void ExecuteQuitButton()
        {
            base.ExecuteQuitButton();
            _numberMelodyNotes.text = "" + 0;
            AnimationController.Instance.ResetSurprise();
            FrogsManager.Instance.AreFrogsBlocked = false;
            MessagesManager.Instance.PressFrogIndication.SetActive(false);
            StartCoroutine(ShowPressFrogIndication(1F));
        }

        public void ExecuteRestartButton()
        {
            base.ExecuteRestartButton();
            _numberMelodyNotes.text = "" + 0;
        }

        public void ExecutePlayAgainButton()
        {
            base.ExecutePlayAgainButton();
            _numberMelodyNotes.text = "" + 0;
            AnimationController.Instance.ResetSurprise();
            _gameManager.Fail = false;
            MessagesManager.Instance.IncorrectAdmiration.gameObject.SetActive(false);
        }

        public IEnumerator ShowMainScreen(float num)
        {
            yield return new WaitForSeconds(num);
            StartCoroutine(FadeElement(_mainScreen, true));
            StartCoroutine(ShowPressFrogIndication(num));
        }

        public IEnumerator ShowPressFrogIndication(float num)
        {
            yield return new WaitForSeconds(num * 1.5F);
            if(MessagesManager.Instance.ShowIndication) StartCoroutine(FadeElement(MessagesManager.Instance.PressFrogIndication, true));
        }

        public void ShowTempoGauge(bool value)
        {
            if(value)
            {
                if((MessagesManager.Instance.ShowIndication) && (_gameManager.IsGameStarted == false))
                {
                    if(MessagesManager.Instance.PressFrogIndication.active) StartCoroutine(FadeElement(MessagesManager.Instance.PressFrogIndication, false));
                    MessagesManager.Instance.ShowIndication = false;
                }
                GaugeIsActive = true;
                MainScreenIsActive = false;
                _gaugeFillCircle.gameObject.GetComponent<Image>().fillAmount = 0;
            }
            else 
            {
                GaugeIsActive = false;
                MainScreenIsActive = true;
                StartCoroutine(FadeElement(_tempoGauge, false));
                _initialTime = Time.time;
            }
        }

        public void ChangeGaugeColor(Color newColor)
        {
            _gaugeMusicIcon.GetComponent<Image>().color = newColor;
            _gaugeFillCircle.GetComponent<Image>().color = newColor;
        }

        public void DetermineGaugeColor(int num)
        {
            switch(num)
            {
                case 0:
                    ChangeGaugeColor(_redColor);
                    break;
                case 1:
                    ChangeGaugeColor(_blueColor);
                    break;
                case 2:
                    ChangeGaugeColor(_yellowColor);
                    break;
                case 3:
                    ChangeGaugeColor(_purpleColor);
                    break;
                case 4:
                    ChangeGaugeColor(_brownColor);
                    break;
                case 5:
                    ChangeGaugeColor(_blackColor);
                    break;
            }
        }

        public void IncrementGauge()
        {
            if(GaugeIsActive)
            {
                _gaugeFillCircle.gameObject.GetComponent<Image>().fillAmount += _tempoAmount * Time.deltaTime; 
            }
        }

        public void CheckInactiveTime()
        {
            if((Time.time - _initialTime > _inactiveDuration) && (!GaugeIsActive)  && (MainScreenIsActive) && (_mainScreen.active == false) && (_gameManager.IsGameStarted == false))
            {
                StartCoroutine(FadeElement(_mainScreen, true));
            }
            else if(GaugeIsActive && !_tempoGauge.active)
            {
                StartCoroutine(FadeElement(_tempoGauge, true));
                if(_mainScreen.active && (_gameManager.IsGameStarted == false) && !MainScreenIsActive) StartCoroutine(FadeElement(_mainScreen, false));
            }
        }
    } 
}

*/