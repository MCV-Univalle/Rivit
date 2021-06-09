using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Home
{
    public class SpeechBubble : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI speech;
        private Dialogue _currentDialogue;
        private int _index = 0;
        private IEnumerator _typingProccess;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _index++;
                DisplayCurrentSentence();
                if (_typingProccess != null)
                {
                    StopCoroutine(_typingProccess);
                    speech.text = _currentDialogue.sentences[_index].text;
                }
            }
            
        }

        public void StartConversation(Dialogue dialogue)
        {
            this.gameObject.SetActive(true);
            _currentDialogue = dialogue;
            _index = 0;
            DisplayCurrentSentence();
        }

        private void DisplayCurrentSentence()
        {
            speech.text = "";
            string sentence = ProccesateSentence(_currentDialogue.sentences[_index].text);
            _typingProccess = TypeSentence(sentence);
            StartCoroutine(_typingProccess);
        }

        private string ProccesateSentence(string sentence)
        {
            //for (int i = 0; i < sentence.Length; i++)
            //{
            //    switch(sentence[i])
            //    {
            //        case '&':
            //            sentence = sentence.Replace("&", UserDataManager.PlayerName);
            //            break;
            //        case '@':
            //            sentence = sentence.Replace("@", UserDataManager.Email);
            //            break;
            //    }
            //}
            return sentence;
        }

        private IEnumerator TypeSentence(string sentence)
        {
            yield return new WaitForSeconds(0.02f);
            foreach (char character in sentence.ToCharArray())
            {

                speech.text += character;
                //_audioSourceTyping.Play();
                yield return new WaitForSeconds(0.02f);
            }
            _typingProccess = null;
        }
    }
}