using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Home
{
    public class RockPaperScissors : MonoBehaviour
    {
        [SerializeField]
        private GameObject _rockPaperScissorsButtonPrefab;
        [SerializeField]
        private Sprite[] _images;
        private int _playersElection;
        private int _remosElections;
        [SerializeField]
        private Dialogue _playerVictory;
        [SerializeField]
        private Dialogue _playerFail;
        [SerializeField]
        private Dialogue _tieGame;

        public void GenerateButtons()
        {
            DestroyButtons();
            UIManager.Instance.FadeBlackPanel(true);
            int[] numbList = {0, 1, 2};
            foreach(int i in numbList)
            {
                GameObject go = Instantiate(_rockPaperScissorsButtonPrefab) as GameObject;
                go.transform.SetParent(UIManager.Instance.RockPaperSccissorsLayout.transform, false);
                go.transform.Find("Image").GetComponent<Image>().sprite = _images[i];
                go.GetComponent<Button>().onClick.AddListener(() => ChooseOption(numbList[i]));
            }
            UIManager.Instance.RockPaperSccissorsLayout.gameObject.SetActive(true);
        }

        public void ChooseOption(int num)
        {
            UIManager.Instance.TargetObject.gameObject.SetActive(false);
            DestroyButtons();
            ConversationManager.Instance.IsLocked = false;
            ConversationManager.Instance.Index = 0;
            _playersElection = num;
            int randNum = Random.Range(0, 3);
            _remosElections = randNum;
            ShowResults();
            if(_playersElection != _remosElections)
            {
                Debug.Log("Qué pasa");
                if(VerifyWinner(_playersElection, _remosElections))
                ConversationManager.Instance.CurrentDialogue = _playerVictory;
                else
                ConversationManager.Instance.CurrentDialogue = _playerFail;
            }
            else ConversationManager.Instance.CurrentDialogue = _tieGame;
            ConversationManager.Instance.AdvanceConversation();
            ConversationManager.Instance.IsLocked = false;
        }

        public bool VerifyWinner(int player, int remo)
        {
            if(((player == 2) && (remo == 1)) || ((player == 1) && (remo == 0)) || ((player == 0) && (remo == 2)))
            return true;
            else return false;
        }

        public void ShowResults()
        {
            DestroyButtons();
            GameObject go = Instantiate(_rockPaperScissorsButtonPrefab) as GameObject;
            go.transform.SetParent(UIManager.Instance.RockPaperSccissorsLayout.transform, false);
            go.transform.Find("Image").GetComponent<Image>().sprite = _images[_playersElection];
            go.GetComponent<Button>().interactable = false;
            go.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Jugador:";

            go = Instantiate(_rockPaperScissorsButtonPrefab) as GameObject;
            go.transform.SetParent(UIManager.Instance.RockPaperSccissorsLayout.transform, false);
            go.transform.Find("Image").GetComponent<Image>().sprite = _images[_remosElections];
            go.GetComponent<Button>().interactable = false;
            go.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Remo:";

        }

        public void DestroyButtons()
            {
                Transform children = UIManager.Instance.RockPaperSccissorsLayout.transform;
                foreach(Transform child in children)
                {
                    GameObject var = child.gameObject;
                    Destroy(var);
                }
            }
    }
}
