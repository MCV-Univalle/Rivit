using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Home
{
    public class QuestionManager : MonoBehaviour
    {
        //Singleton implementation
        private static QuestionManager _instance;
        public static QuestionManager Instance
        {
            get
            {
                //Logic to create the instance
                if(_instance == null)
                {
                    GameObject go = new GameObject("QuestionManager");
                    go.AddComponent<QuestionManager>();
                    _instance = go.GetComponent<QuestionManager>(); 
                }
                return _instance;
            }
        }
        public GameObject choiceButtonPrefab;
        [SerializeField]
        private Question _question;

        void Awake()
        {
            _instance = this;
        }

        public void MakeQuestion()
        {
            UIManager.Instance.DialogueText.text = "";
            string sentenceText = _question.sentence;
            ConversationManager.Instance.StartTyping(sentenceText, Action.ShowChoices);

            foreach (var item in _question.choices)
            {
                GameObject go = Instantiate(choiceButtonPrefab) as GameObject;
                go.transform.SetParent(UIManager.Instance.ChoiceButtonsContainer, false);

                string choiceText = item.text;
                go.GetComponentInChildren<TextMeshProUGUI>().text = choiceText;

                Dialogue dialogue = item.dialogue;
                go.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(StartNewDialogue(dialogue)));
            }
        }

        public void DestroyChoices()
        {
            Transform children = UIManager.Instance.ChoiceButtonsContainer.transform;
            foreach(Transform child in children)
            {
                GameObject choice = child.gameObject;
                Destroy(choice);
            }
        }

        public IEnumerator StartNewDialogue(Dialogue dialogue)
        {
            UIAudio.Instance.PlayConfirmationClip();
            yield return new WaitForSeconds(0.1f);
            UIManager.Instance.ShowChoices(false);
            ConversationManager.Instance.Index = 0;
            ConversationManager.Instance.IsLocked = false;
            ConversationManager.Instance.CurrentDialogue = dialogue;
            ConversationManager.Instance.AdvanceConversation();
            DestroyChoices();
        }
    }
}