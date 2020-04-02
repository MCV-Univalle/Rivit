using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Home
{
    public class ConversationManager : MonoBehaviour
    {
        //Singleton implementation
        private static ConversationManager _instance;
        public static ConversationManager Instance
        {
            get
            {
                //Logic to create the _instance
                if(_instance == null)
                {
                    GameObject go = new GameObject("ConversationManager");
                    go.AddComponent<ConversationManager>();
                    _instance = go.GetComponent<ConversationManager>();
                }
                return _instance;
            }
        }

        public int Index {get; set;}
        public bool IsShowingTipying { get; set; }
        public bool IsLocked { get; set; }
        public bool IsSpeaking { get; set; }

        [SerializeField]
        private AudioSource _audioSourceTyping;
        [SerializeField]
        private Dialogue _currentDialogue;
        [SerializeField]
        private List<Dialogue> _conversationsList = new List<Dialogue>();  
        public Dialogue CurrentDialogue { set{ _currentDialogue = value; } }
        private IEnumerator _typingCoroutine;

        void Awake()
        {
            _instance = this;
            Index = 0;
            IsShowingTipying = false;
            IsLocked = false;
            IsSpeaking = false;
        }


        void Update()
        {
            if(Input.GetMouseButtonDown(0) && (_currentDialogue != null))
            {
                if((IsSpeaking) && (!IsLocked) && (!IsShowingTipying))
                {
                    UIManager.Instance.ChangeDialogue();
                    AdvanceConversation();
                }
                else if(IsShowingTipying)
                {
                    string displayText = "";
                    Action action = new Action();
                    displayText = PrepareDialogue(displayText, out action);
                    StopCoroutine(_typingCoroutine);
                    FinishTyping(displayText, action);
                }
            }
        }

        public string PrepareDialogue(string displayText, out Action action)
        {
            string sentence = _currentDialogue.sentences[Index].text;
            action = _currentDialogue.sentences[Index].action;
            displayText = AnalyseText(sentence);
            return displayText;
        }

        public IEnumerator MakeRandomConversation()
        {
            yield return new WaitForSeconds(0.03f);
            AnimationManager.Instance.ShowMouthSpeaking(true);
            int sizeOfList = _conversationsList.Count;
            int num = Random.Range(0, sizeOfList);
            _currentDialogue = _conversationsList[num];
            Index = 0;
            AdvanceConversation();
        }

        public void AdvanceConversation() 
        {
            if(_currentDialogue != null)
            {
                if(Index < _currentDialogue.sentences.Length) //If there is more reminder dialogues, increase index
                {
                string displayText = "";
                Action action = new Action();
                displayText = PrepareDialogue(displayText, out action);
                UIManager.Instance.DialogueText.text = "";
                StartTyping(displayText, action);
                }
                else 
                {
                    FinishConversation();
                }
            }
        }

        public void FinishConversation()
        {
            Index = 0;
            UIManager.Instance.DialogueText.text = "";
            UIManager.Instance.ChangeBubbleDisplay(false);
            UIManager.Instance.MenuButtonsContainer.gameObject.SetActive(true);
            IsSpeaking = false;
            _currentDialogue = null;
            StartCoroutine(UIManager.Instance.BlockUI());
        }

        public void PrepareToAvance()
        {
            IsLocked = false;
            if(Index < _currentDialogue.sentences.Length - 1) 
            StartCoroutine(UIManager.Instance.FadeElement(UIManager.Instance.AdvanceIcon, true));
        }

        public string CheckInput(string sentence, string inputVar)
        { 
            if(inputVar == "name")
            {
                sentence = sentence.Replace("@[name]", GeneralGameManager.Instance.PlayerName);
            }
            return sentence;
        }

        public string AnalyseText(string sentence)
        {
            for(int i = 0; i < sentence.Length; i++)
            {
                if((sentence[i] == '@') && (sentence[i+1] == '['))
                {
                    i += 2;
                    string inputVar = "";
                    while(sentence[i] != ']')
                    {
                        inputVar += sentence[i];
                        i++;
                    }
                    sentence = CheckInput(sentence, inputVar);
                }
            }
            return sentence;
        } 

        public void StartTyping(string sentence, Action action)
        {
            AnimationManager.Instance.ShowMouthSpeaking(true);
            IsShowingTipying = true;
            _typingCoroutine = DisplayTypingEffect(sentence, action);
            StartCoroutine(_typingCoroutine);
        }

        public IEnumerator DisplayTypingEffect(string sentence, Action action)
        {
            yield return new WaitForSeconds(0.02f);
            foreach (char character in sentence.ToCharArray())
            {
                UIManager.Instance.DialogueText.text += character;
                _audioSourceTyping.Play();
                yield return new WaitForSeconds(0.02f);
                
            }
            FinishTyping(sentence, action);
        }

        public void FinishTyping(string sentence, Action action)
        {
            UIManager.Instance.DialogueText.text = sentence;
            IsShowingTipying = false;
            string exec = action.ToString();
            ActionsManager.Instance.TriggerEvent(exec);
            AnimationManager.Instance.ShowMouthSpeaking(false);
            if(sentence != "") Index++;
        }
    }

}
