using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SpaceShip
{
    public delegate void Notify();
    public class EnergyControl : MonoBehaviour
    {
        private List<int> numberStack = new List<int>();
        private int[] correctOrder = new int[5];
        private int _index = 1;
        private AudioSource _audioSource;

        [SerializeField] private Image energyGauge;
        [SerializeField] private EnergyControlButton[] buttonsArray = new EnergyControlButton[5];
        [SerializeField] private List<Color> colorLists = new List<Color>(3);

        [SerializeField] private AudioSource xD;

        public static event Notify energyRunOut;

        [Inject(Id = "SFXManager")] AudioManager sfxManager;

        private bool isGaugeCharged;

        public float EnergyDecreaseRate { get; set; }

        void Start()
        {
            this._audioSource = GetComponent<AudioSource>();
            GenerateOrder();
        }

        void Update()
        {
            if (energyGauge.fillAmount == 0 && energyRunOut != null)
                energyRunOut();
            else if (energyGauge.fillAmount > 0 && isGaugeCharged == false)
                energyGauge.fillAmount -= Time.deltaTime * EnergyDecreaseRate;
            else if (energyGauge.fillAmount < 1 && isGaugeCharged == true)
                energyGauge.fillAmount += Time.deltaTime * 2.5F;
        }

        private void OnEnable()
        {
            energyGauge.fillAmount = 1;
            xD.Play();
            this.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            LeanTween.alphaCanvas(this.gameObject.GetComponent<CanvasGroup>(), 1F, 0.25F);
            isGaugeCharged = false;
            UnlockButtons();
        }

        private void OnDestroy()
        {
            energyRunOut = null;
        }

        public bool ValidateInput(int num)
        {
            
            if (num == _index)
            {
                if(_index == 5)
                {
                    this.gameObject.GetComponent<Image>().color = colorLists[1];
                    ChangeButtonsColor(colorLists[1]);
                    _audioSource.clip = sfxManager.GetAudio("Correct");
                    _audioSource.Play();
                    StartCoroutine(StartNewSequence(3.5F));
                    isGaugeCharged = true;
                    xD.Stop();
                }
                else
                {
                    _index++;
                    _audioSource.clip = sfxManager.GetAudio("Beep");
                    _audioSource.Play();
                }
                    
                return true;
            }
            else
            {
                this.gameObject.GetComponent<Image>().color = colorLists[2];
                ChangeButtonsColor(colorLists[2]);
                _audioSource.clip = sfxManager.GetAudio("Wrong");
                _audioSource.Play();
                StartCoroutine(StartNewSequence(0.75F));
                return false;
            }
        }

        private void LockButtons()
        {
            foreach (var item in buttonsArray)
            {
                item.gameObject.GetComponent<Button>().interactable = false;
                item.gameObject.GetComponent<Image>().raycastTarget = false;
            }
        }

        private void UnlockButtons()
        {
            foreach (var item in buttonsArray)
            {
                item.gameObject.GetComponent<Button>().interactable = true;
                item.gameObject.GetComponent<Image>().raycastTarget = true;
            }
        }

        private void ChangeButtonsColor(Color tempColor)
        {
            foreach (var item in buttonsArray)
            {
                item.gameObject.GetComponent<Image>().color = tempColor;
            }
        }

        public IEnumerator StartNewSequence(float waitTime)
        {
            LockButtons();
            yield return new WaitForSeconds(waitTime);
            UnlockButtons();
            GenerateOrder();
            isGaugeCharged = false;
            xD.Play();
            _index = 1;
        }



        public void GenerateOrder()
        {
            this.gameObject.GetComponent<Image>().color = colorLists[0];
            ChangeButtonsColor(colorLists[0]);
            _index = 1;
            numberStack = new List<int>();
            var tempList = new List<int> { 1, 2, 3, 4, 5 };
            for (int i = 0; i < correctOrder.Length; i++)
            {
                int num = Random.Range(0, tempList.Count);
                buttonsArray[i].NumberIdText.text = tempList[num] + "";
                tempList.RemoveAt(num);
            }
        }
    }
}