using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Home
{
    public enum Action
    {
        NoAction,
        Silence,
        AskName,
        ShowChoices,
        RandomConversation,
        PlayRockPaperScissors,
        ShowRPSLayout
    }

    public class ActionsManager : MonoBehaviour
    {
        private static ActionsManager _instance;

        public static ActionsManager Instance
        {
            get
            {
                //Logic to create the instance
                if(_instance == null)
                {
                    GameObject go = new GameObject("ActionsManager");
                    go.AddComponent<ActionsManager>();
                    _instance = go.GetComponent<ActionsManager>();
                }
                return _instance;
            }
        }
        private Dictionary <string, UnityEvent> eventDictionary;
        [SerializeField]
        private RockPaperScissors _rockPaperScissors;

        void Awake()
        {
            _instance = this;
            eventDictionary = new Dictionary<string, UnityEvent>();
        }

        void Start()
        {
            StartListening("NoAction", PrepareToAvance);
            StartListening("Silence", MakeSilence);
            StartListening("AskName", AskName);
            StartListening("ShowChoices", ShowChoices);
            StartListening("RandomConversation", MakeRandomConversation);
            StartListening("PlayRockPaperScissors", PlayRockPaperScissors);
            StartListening("ShowRPSLayout", ShowRockPapersSccissorLayout);
        }

        void StartListening(string eventName, UnityAction listener)
        {
            UnityEvent thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            eventDictionary.Add(eventName, thisEvent);
        }

        public void TriggerEvent(string eventName)
        {
            UnityEvent evnt = null;
            if(eventDictionary.TryGetValue (eventName, out evnt))
            {
                evnt.Invoke();
            }
            else Debug.Log("Event not found");
        }

        public void PrepareToAvance()
        {
            ConversationManager.Instance.PrepareToAvance();
            
        }

        public void MakeSilence()
        {
            StartCoroutine(WaitAMoment(" .", 3, 0.5F));
        }

        public IEnumerator WaitAMoment(string s, int num, float time)
        {
            ConversationManager.Instance.IsLocked = true;
            for(int i = 0; i < num; i++)
            {
                yield return new WaitForSeconds(time);
                UIManager.Instance.DialogueText.text += s;
            }
            ConversationManager.Instance.IsLocked = false;
        }

        public void AskName()
        {
            UIManager.Instance.ShowNameInput();
        }
        public void ShowChoices()
        {
            UIManager.Instance.ShowChoices(true);
        }

        public void MakeRandomConversation()
        {
            StartCoroutine(ConversationManager.Instance.MakeRandomConversation());
        }
        public void PlayRockPaperScissors()
        {
            _rockPaperScissors.GenerateButtons();
            ConversationManager.Instance.IsLocked = true;
            UIManager.Instance.TargetObject = UIManager.Instance.RockPaperSccissorsLayout;
        }
        public void ShowRockPapersSccissorLayout()
        {
            if(UIManager.Instance.RockPaperSccissorsLayout.gameObject.active)
            {
                UIManager.Instance.FadeOutTargetObject();
                UIManager.Instance.FadeBlackPanel(false);
            }
            else UIManager.Instance.RockPaperSccissorsLayout.gameObject.SetActive(true); 
        }
    }
}