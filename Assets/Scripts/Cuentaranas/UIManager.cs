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
        private GameObject _questionPanel;
        [SerializeField]
        private GameObject _correctPanel;  
        [SerializeField]
        private TMP_InputField _numberInputField;
        [SerializeField]
        private TextMeshProUGUI _correctText;
        [SerializeField]
        private TextMeshProUGUI _numberText;
        [SerializeField]
        public TextMeshProUGUI _userAnswer; //Corregir

        [SerializeField]
        private TextMeshProUGUI _timerText;
        public TextMeshProUGUI TimerText {get {return _timerText;}}

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

        public void TemporalStartButton()
        {
            GameManager.Instance.StartGame();
        }

        public void Accept()
        {
            _questionPanel.gameObject.SetActive(false);
            int num = int.Parse(_numberInputField.text);
            _correctText.text = GameManager.Instance.CompareUserInput(num);
            //_numberText.text = "" + FrogsManager.Instance.CountFrogs();
            int realNumber = FrogsManager.Instance.CountFrogs();
            Debug.Log(realNumber);
            //StartCoroutine(ShowCorrectPanel());
            FinalFrogsCounter.Instance.PutActualFrogs(realNumber);
            _numberInputField.text = "";
            _userAnswer.text = "" + num;
            _userAnswer.gameObject.SetActive(true);
        }

        public IEnumerator ShowCorrectPanel()
        {
            _correctPanel.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            _correctPanel.gameObject.SetActive(false);
            GameManager.Instance.StartNewIteration();
        }

        public IEnumerator MakeQuestion()
        {
            yield return new WaitForSeconds(2f);
            _questionPanel.gameObject.SetActive(true);
            _numberInputField.GetComponent<TMP_InputField>().Select();
            _numberInputField.GetComponent<TMP_InputField>().ActivateInputField();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }   
}
