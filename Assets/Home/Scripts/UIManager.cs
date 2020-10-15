using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Home
{
    public class UIManager : MonoBehaviour
    {
        //Singleton implementation
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
        private AudioManager bgmManager;


        public bool IsBlocked {get; set;}
        [SerializeField]
        private GameObject _sceneSwitcher;
        [SerializeField]
        private GameObject _whiteScreen;
        [SerializeField]
        private GameObject _blackPanel;
        [SerializeField]
        private GameObject _inputNameLayout;
        [SerializeField]
        private TMP_InputField _inputNameField;
        [SerializeField]
        private GameObject _speechBubble;
        [SerializeField]
        private GameObject _speechButton;
        [SerializeField]
        private GameObject _gameSelectionScreen;
        [SerializeField]
        private GameObject _menuButtonsContainer;
        [SerializeField]
        private GameObject _advanceIcon;
        [SerializeField]
        private TextMeshProUGUI _dialogueText;
        [SerializeField]
        private GameObject _rockPaperSccissorsLayout;
        [SerializeField]
        private Transform _choiceButtonsContainer;
        public GameObject SpeechBubble { get { return _speechBubble;} }
        public GameObject AdvanceIcon { get { return _advanceIcon;} }
        public Transform ChoiceButtonsContainer { get{ return _choiceButtonsContainer; } }
        public TextMeshProUGUI DialogueText { get{ return _dialogueText; } }
        public GameObject RockPaperSccissorsLayout {get {return _rockPaperSccissorsLayout;}}
        public GameObject MenuButtonsContainer {get {return _menuButtonsContainer;}}

        private GameObject _targetObject;
        public GameObject TargetObject {get {return _targetObject;} set { _targetObject = value;}}

        void Awake()
        {
            _instance = this;
            IsBlocked = false;
        }

        private void Start()
        {
            bgmManager.PlayAudio("Home");
            bgmManager.FadeIn(0.5F);
        }

        public IEnumerator BlockUI()
        {
            IsBlocked = true;
            yield return new WaitForSeconds(0.25F);
            IsBlocked = false;
        }

        public IEnumerator FadeElement(GameObject go, bool value)
        {
            yield return new WaitForSeconds(0.06F);
            if(value) 
            go.SetActive(true);
            else
            go.gameObject.GetComponent<UIFader>().FadeOut(0, false);
        }

        public void FadeOutTargetObject()
        {
            StartCoroutine(FadeElement(_targetObject, false));
        }

        public void FadeBlackPanel(bool isActive)
        {
            StartCoroutine(FadeElement(_blackPanel, isActive));
        } 
        
        public IEnumerator DisplayDesactiveAnimation(GameObject go)
        {
            yield return new WaitForSeconds(0.06F);
            go.gameObject.GetComponent<Animator>().SetTrigger("desactive");
        }

        public void ShowNameInput()
        {
            ConversationManager.Instance.IsLocked = true;
            StartCoroutine(FadeElement(_blackPanel, true));
            StartCoroutine(FadeElement(_inputNameLayout, true));
        }
        public void SaveNameInput()
        {
            UIAudio.Instance.PlayConfirmationClip();
            _blackPanel.gameObject.SetActive(false);
            _inputNameLayout.gameObject.SetActive(false);
            GeneralGameManager.Instance.PlayerName = _inputNameField.text;
            GeneralGameManager.Instance.IsFirstTime = false;
            GeneralGameManager.Instance.SaveData();
            ConversationManager.Instance.IsLocked = false;
            ChangeDialogue();
            ConversationManager.Instance.AdvanceConversation();
        }

        public void ChangeBubbleDisplay(bool value)
        {
            StartCoroutine(FadeElement(_speechBubble, value));
        }
        public void ShowChoices(bool value)
        {
            StartCoroutine(FadeElement(_blackPanel, value));
            StartCoroutine(FadeElement(_choiceButtonsContainer.gameObject, value));
        }
        public void ChangeDialogue()
        {
            _speechBubble.GetComponent<Animator>().SetTrigger("TextFade");
            if(_advanceIcon.gameObject.active) StartCoroutine(FadeElement(_advanceIcon, false));
        }
        public void MakeQuestion()
        {
            if((!ConversationManager.Instance.IsSpeaking) && !IsBlocked)
            {   
                StartCoroutine(BlockUI());
                ChangeBubbleDisplay(true);
                ConversationManager.Instance.IsSpeaking = true;
                ConversationManager.Instance.IsLocked = true;
                QuestionManager.Instance.MakeQuestion(); 
                StartCoroutine(FadeElement(_menuButtonsContainer, false));
            }
        }
        public void ExecuteGamesButton()
        {
            if(!IsBlocked)
            {
                StartCoroutine(BlockUI());
                bool value = true;
                StartCoroutine(FadeElement(_gameSelectionScreen, value));
                StartCoroutine(FadeElement(_menuButtonsContainer, !value));
                UIAudio.Instance.PlayConfirmationClip();
            }
        }

        public void ExecuteCloseButton()
        {
            if(!IsBlocked)
            {
                StartCoroutine(BlockUI());
                bool value = false;
                StartCoroutine(FadeElement(_gameSelectionScreen, value));
                StartCoroutine(FadeElement(_menuButtonsContainer, !value));
                StartCoroutine(DisplayDesactiveAnimation(_gameSelectionScreen));
                UIAudio.Instance.PlayCancelClip();
            }
        }

        public void SwitchScene(string sceneName)
        {
            UIAudio.Instance.PlayConfirmationClip();
            bgmManager.FadeOut(0.5F);
            _whiteScreen.gameObject.SetActive(true);
            StartCoroutine(_sceneSwitcher.GetComponent<SceneSwitcher>().GoToGame(sceneName));
        }
    }   
}
