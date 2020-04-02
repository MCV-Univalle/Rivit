using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CoroMelodia
{
    public class MessagesManager : MonoBehaviour
    {
        private static MessagesManager _instance;
        public static MessagesManager Instance
        {
            get
            {
                //Logic to create the instance
                if(_instance == null)
                {
                    GameObject go = new GameObject("MessagesManager");
                    go.AddComponent<MessagesManager>();
                    _instance = go.GetComponent<MessagesManager>();
                }
                return _instance;
            }
        }
        [SerializeField]
        private string _message;
        [SerializeField]
        private GameObject _messageBubble;
        [SerializeField]
        private GameObject _pressFrogIndication;
        public GameObject PressFrogIndication {get {return _pressFrogIndication;}}
        public bool ShowIndication {get; set;}
        [SerializeField]
        private GameObject _correctBubble;
        [SerializeField]
        private GameObject _incorrectAdmiration;
        public GameObject IncorrectAdmiration {get {return _incorrectAdmiration;}}
        // Start is called before the first frame update
        void Awake()
        {
            _instance = this;
            ShowIndication = true;
        }

        public void ShowPreparationMessage()
        {
            _message = "Memoriza esta melodía";
            StartCoroutine(ShowMessageBubble(true));
        }

        public void ShowCongratulationMessage()
        {
            int randomNum = Random.Range(0, 2);
            if(randomNum == 0)
            _message = "Bien hecho";
            else 
            _message = "Excelente";
            StartCoroutine(ShowMessageBubble(true));
        }

        public IEnumerator ShowMessageBubble(bool value)
        {
            if(value == false)
            {
                _messageBubble.GetComponent<Animator>().SetTrigger("desactive");
                yield return new WaitForSeconds(0.5F);
            }
            _messageBubble.gameObject.SetActive(value);
            _messageBubble.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _message;
            yield return null;;
        }

        public void ShowCorrectFrog(GameObject go)
        {
            _incorrectAdmiration.gameObject.SetActive(true);
            Vector3 pos = go.GetComponent<Renderer>().transform.position;
            pos.x += 0.5F;
            pos.y += 0.55F;
            _incorrectAdmiration.GetComponent<Renderer>().transform.position = pos;
        }
    }   
}
