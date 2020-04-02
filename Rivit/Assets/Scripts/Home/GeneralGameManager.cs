using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Home
{
    public class GeneralGameManager : MonoBehaviour
    {
        //Singleton implementation
        private static GeneralGameManager _instance;
        public static GeneralGameManager Instance
        {
            get
            {
                //Logic to create the instance
                if(_instance == null)
                {
                    GameObject go = new GameObject("GeneralGameManager");
                    go.AddComponent<GeneralGameManager>();
                    _instance = go.GetComponent<GeneralGameManager>();
                }
                return _instance;
            }
        }

        public string PlayerName{get; set;}
        public bool IsFirstTime {get; set;}
        public bool IsBootingGame {get; set;}
        [SerializeField]
        private Dialogue _firstTimeDialogue;
        [SerializeField]
        private Dialogue _welcomeDialogue;

        void Awake()
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("GeneralGameManager");
            if(objs.Length > 1) Destroy(this.gameObject);
            else
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
                IsBootingGame = true;
            }
            IsFirstTime = true;
            LoadData();
        }

        // Start is called before the first frame update
        void Start()
        {
            VerifyFirstTime();
            UIManager.Instance.SpeechBubble.gameObject.SetActive(true);
            VerifyBootingGame();
            ConversationManager.Instance.AdvanceConversation();
            IsBootingGame = false;
        }

        public void VerifyBootingGame()
        {
            if(IsBootingGame)
            {
                UIManager.Instance.MenuButtonsContainer.gameObject.SetActive(false);
                UIManager.Instance.SpeechBubble.gameObject.SetActive(true);
                ConversationManager.Instance.IsSpeaking = true;
            }
        }

        public void VerifyFirstTime()
        {
            if(IsFirstTime)
            ConversationManager.Instance.CurrentDialogue = _firstTimeDialogue;
            else 
            ConversationManager.Instance.CurrentDialogue = _welcomeDialogue;
        }

        public void SaveData()
        {
            PlayerData data = new PlayerData();
            data.playerName = PlayerName;
            data.isFirstTime = IsFirstTime;
            string json = JsonUtility.ToJson(data);
            Debug.Log(json);
            PlayerPrefs.SetString("PlayerData", json);
            PlayerPrefs.Save();
        }

        public void LoadData()
        {
            string jsonString = PlayerPrefs.GetString("PlayerData");
            Debug.Log(jsonString);
            if(jsonString != "")
            {
                PlayerData data = new PlayerData();
                data = JsonUtility.FromJson<PlayerData>(jsonString);
                PlayerName = data.playerName;
                IsFirstTime = data.isFirstTime;
            }
            else Debug.Log("Save data not found!");
            }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
