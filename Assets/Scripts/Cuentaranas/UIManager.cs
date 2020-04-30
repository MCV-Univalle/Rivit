using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Cuentaranas
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
        private GameObject _lifesCosito;
        public GameObject LifesCosito {get {return _lifesCosito;}}
        [SerializeField]
        private TextMeshProUGUI _lifesCounter;

        public TextMeshProUGUI LifesCounter {get {return _lifesCounter;}}

        [SerializeField]
        private GameObject _questionPanel;
        [SerializeField]
        private FinalFrogsCounter _finalFrogsCounter; 
        [SerializeField]
        private TMP_InputField _numberInputField;
        [SerializeField]
        private GameObject _acceptButton;

        [SerializeField]
        private TextMeshProUGUI _timerText;
        public TextMeshProUGUI TimerText {get {return _timerText;}}
        [SerializeField]
        private TextMeshProUGUI _correctText;

        void Awake()
        {
            _instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            _gameManager = GameManager.Instance;
            base.Start();
        }

        public void AcceptUserInput()
        {
            int num = int.Parse(_numberInputField.text);
            int realNumber = GameManager.Instance.CompareUserInput(num);
            StartCoroutine(FadeElement(_acceptButton, false));
            _finalFrogsCounter.PutActualFrogs(realNumber);
            _numberInputField.interactable = false;
            UIAudio.Instance.PlayConfirmationClip();
        }

        public void ChangeCorrecText()
        {
            if(GameManager.Instance.IsCorrect)
            _correctText.text = "¡Correcto!";
            else
            _correctText.text = "Fallaste :(";
            StartCoroutine(FadeElement(_correctText.gameObject, true));
        }

        public void FinishQuestion()
        {
            StartCoroutine(FadeElement(_questionPanel, false));
            StartCoroutine(FadeElement(_topElements, true));
            _blurSuperfice.gameObject.SetActive(false);
            _finalFrogsCounter.IsCountingFinished = false;
            GameManager.Instance.StartNewIteration();
            _numberInputField.text = "";
        }

        public IEnumerator MakeQuestion()
        {
            _correctText.gameObject.SetActive(false);
            _numberInputField.interactable = true;
            yield return new WaitForSeconds(2f);
            StartCoroutine(FadeElement(_acceptButton, true));
            _blurSuperfice.gameObject.SetActive(true);
            StartCoroutine(FadeElement(_questionPanel, true));
            StartCoroutine(FadeElement(_topElements, false));
            yield return new WaitForSeconds(0.1f);
            _numberInputField.gameObject.SetActive(true);
            _numberInputField.GetComponent<TMP_InputField>().ActivateInputField();
            _numberInputField.GetComponent<TMP_InputField>().Select();
        }

        void Update()
        {
            if(_finalFrogsCounter.IsCountingFinished)
            FinishQuestion();
        }
    }   
}
