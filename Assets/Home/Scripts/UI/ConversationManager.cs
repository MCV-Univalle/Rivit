//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;

//namespace Home
//{
//    public class ConversationManager : MonoBehaviour
//    {

//        public int Index { get; set; }
//        public bool IsShowingTipying { get; set; }
//        public bool IsLocked { get; set; }
//        public bool IsSpeaking { get; set; }

//        [SerializeField]
//        private AudioSource _audioSourceTyping;
//        [SerializeField]
//        private Dialogue _currentDialogue;
//        [SerializeField]
//        private List<Dialogue> _conversationsList = new List<Dialogue>();
//        public Dialogue CurrentDialogue { set { _currentDialogue = value; } }
//        private IEnumerator _typingCoroutine;

//        void Awake()
//        {
//            Index = 0;
//            IsShowingTipying = false;
//            IsLocked = false;
//            IsSpeaking = false;
//        }


//        void Update()
//        {
//            if (Input.GetMouseButtonDown(0) && (_currentDialogue != null))
//            {
//                if ((IsSpeaking) && (!IsLocked) && (!IsShowingTipying))
//                {
//                    //UIManager.Instance.ChangeDialogue();
//                    AdvanceConversation();
//                }
//                else if (IsShowingTipying)
//                {
//                    string displayText = "";
//                    displayText = PrepareDialogue(displayText);
//                    StopCoroutine(_typingCoroutine);
//                    FinishTyping(displayText);
//                }
//            }
//        }

//        public string PrepareDialogue(string displayText)
//        {
//            string sentence = _currentDialogue.sentences[Index].text;
//            action = _currentDialogue.sentences[Index].action;
//            displayText = AnalyseText(sentence);
//            return displayText;
//        }

//        public void AdvanceConversation()
//        {
//            if (_currentDialogue != null)
//            {
//                if (Index < _currentDialogue.sentences.Length)
//                {
//                    string displayText = "";
//                    displayText = PrepareDialogue(displayText);
//                    //UIManager.Instance.DialogueText.text = "";
//                    PrepareTyping(displayText);
//                }
//                else
//                {
//                    FinishConversation();
//                }
//            }
//        }

//        public void FinishConversation()
//        {
//            Index = 0;
//            IsSpeaking = false;
//            _currentDialogue = null;
//        }

//        public void PrepareToAvance()
//        {
//            IsLocked = false;
//            if (Index < _currentDialogue.sentences.Length - 1)
//                StartCoroutine(UIManager.Instance.FadeElement(UIManager.Instance.AdvanceIcon, true));
//        }

//        public string CheckInput(string sentence, string inputVar)
//        {
//            if (inputVar == "name")
//            {
//                sentence = sentence.Replace("@[name]", GeneralGameManager.Instance.PlayerName);
//            }
//            return sentence;
//        }

//        public string AnalyseText(string sentence)
//        {
//            for (int i = 0; i < sentence.Length; i++)
//            {
//                if ((sentence[i] == '@') && (sentence[i + 1] == '['))
//                {
//                    i += 2;
//                    string inputVar = "";
//                    while (sentence[i] != ']')
//                    {
//                        inputVar += sentence[i];
//                        i++;
//                    }
//                    sentence = CheckInput(sentence, inputVar);
//                }
//            }
//            return sentence;
//        }

//        public void PrepareTyping(string sentence)
//        {
//            animationManager.ShowMouthSpeaking(true);
//            IsShowingTipying = true;
//            _typingCoroutine = DisplayTypingEffect(sentence);
//            StartCoroutine(_typingCoroutine);
//        }

//        public IEnumerator DisplayTypingEffect(string sentence)
//        {
//            yield return new WaitForSeconds(0.02f);
//            foreach (char character in sentence.ToCharArray())
//            {
//               // UIManager.Instance.DialogueText.text += character;
//                //_audioSourceTyping.Play();
//                yield return new WaitForSeconds(0.02f);

//            }
//            Finish();
//        }

//        private void Finish()
//        {
//            IsShowingTipying = false;
//            animationManager.ShowMouthSpeaking(false);
//            if (sentence != "") Index++;
//        }

//    }

//}