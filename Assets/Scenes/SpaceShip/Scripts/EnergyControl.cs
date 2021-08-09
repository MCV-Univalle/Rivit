using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SpaceShip
{
    public delegate void Notify();
    public abstract class EnergyControl : MonoBehaviour
    {

        protected AudioSource _audioSource;

        [SerializeField] protected Image energyGauge;
        [SerializeField] protected GameObject extraPanel;
        [SerializeField] protected EnergyControlButton[] buttonsArray = new EnergyControlButton[5];
        [SerializeField] protected List<Color> colorLists = new List<Color>(3);

        [SerializeField] protected AudioSource tempoSound;

        public static event Notify energyRunOut;
        public static event Notify energyCharged;

        [Inject(Id = "SFXManager")] protected AudioManager _sfxManager;
        protected SpaceShipGameManager _gameManager;

        private bool isGaugeCharged;

        private float passedTime = 0f;

        public float EnergyDecreaseRate { get; set; }
        public bool IsGaugeCharged { get => isGaugeCharged; set => isGaugeCharged = value; }

        public bool IsActive { get; set; }

        [Inject]
        private void Init(GameManager gameManager)
        {
            _gameManager = gameManager as SpaceShipGameManager;
        }

        void Start()
        {
            this._audioSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            if (energyGauge.fillAmount == 0 && energyRunOut != null)
                energyRunOut();
            else if (energyGauge.fillAmount > 0 && isGaugeCharged == false)
            {
                passedTime += Time.deltaTime;
                energyGauge.fillAmount -= Time.deltaTime * _gameManager.EnergyGaugeDecreaseRate;
            }
            else if (energyGauge.fillAmount < 1 && isGaugeCharged == true)
                ChargeEnergy();
            
                
        }

        private void OnDestroy()    
        {
            energyRunOut = null;
        }

        public void ChargeEnergy()
        {
            energyGauge.fillAmount = 1;
            
            LeanTween.alphaCanvas(this.gameObject.GetComponent<CanvasGroup>(), 0F, 0.15F).setDelay(0.45F);
            LeanTween.delayedCall(this.gameObject, 0.65F, ()=> gameObject.SetActive(false));
        }

        private void OnEnable()
        {
            SetToDefault();
            StartTask();
            passedTime = 0;
        }

        private void SetToDefault()
        {
            energyGauge.fillAmount = 1;
            ChangeColor(colorLists[0]);
            tempoSound.Play();
            this.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            LeanTween.alphaCanvas(this.gameObject.GetComponent<CanvasGroup>(), 1F, 0.15F);
            isGaugeCharged = false;
            UnlockButtons();
        }

        protected void ShowPositiveFeedback()
        {
            _gameManager.AdditionalData.CorrectAnswers++;
            _gameManager.TotalTime += passedTime;
            _gameManager.ContinueWithTask();
            ChangeColor(colorLists[2]);
            _audioSource.clip = _sfxManager.GetAudio("Correct");
            _audioSource.Play();
            isGaugeCharged = true;
            tempoSound.Stop();
        }

        protected void ShowNegativeFeedBack()
        {
            _gameManager.AdditionalData.WrongAnswers++;
            ChangeColor(colorLists[1]);
            _audioSource.clip = _sfxManager.GetAudio("Wrong");
            _audioSource.Play();
        }

        public abstract void ValidateInput(int num);
        public abstract void StartTask();

        

        protected void LockButtons()
        {
            foreach (var item in buttonsArray)
            {
                item.gameObject.GetComponent<Button>().interactable = false;
                item.gameObject.GetComponent<Image>().raycastTarget = false;
            }
        }

        protected void UnlockButtons()
        {
            foreach (var item in buttonsArray)
            {
                item.gameObject.GetComponent<Button>().interactable = true;
                item.gameObject.GetComponent<Image>().raycastTarget = true;
            }
        }

        protected void ChangeColor(Color tempColor)
        {
            this.gameObject.GetComponent<Image>().color = tempColor;
            if(extraPanel != null) extraPanel.gameObject.GetComponent<Image>().color = tempColor;
            foreach (var item in buttonsArray)
            {
                item.gameObject.GetComponent<Image>().color = tempColor;
            }
        }

        public IEnumerator RestartTask(float waitTime)
        {
            LockButtons();
            yield return new WaitForSeconds(waitTime);
            UnlockButtons();
            isGaugeCharged = false;
            tempoSound.Play();
            StartTask();
        }



        
    }
}