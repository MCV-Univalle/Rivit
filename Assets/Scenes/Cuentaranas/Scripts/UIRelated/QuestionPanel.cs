using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;

namespace Cuentaranas
{
    public class QuestionPanel : MonoBehaviour
    {
        [SerializeField] private FrogsCounterGameManager _gameManager;
        [SerializeField] private GameObject _frogPrefab;
        [SerializeField] private GameObject _layoutA;
        [SerializeField] private GameObject _layoutB;
        [SerializeField] private GameObject _layoutC;
        [SerializeField] private GameObject _question;
        [SerializeField] private TMP_InputField _numberInputField;
        [SerializeField] private GameObject _acceptButton;
        [SerializeField] private TextMeshProUGUI _correctText;
        [SerializeField] private GameObject inGamePanel;
        [Inject (Id="SFXManager")] private AudioManager _SFXManager;

        bool _isCorrect = false;

        private void OnEnable()
        {
            ToDefaultValues();
            LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 1, 0.25F);
            _question.transform.localPosition = new Vector3(0, 700, 0);
            LeanTween.moveY(_question.gameObject.GetComponent<RectTransform>(), -175, 0.45F).setEaseInOutCubic();;
        }

        public void ToDefaultValues()
        {
            inGamePanel.SetActive(false);
            gameObject.SetActive(true);
            _acceptButton.gameObject.SetActive(true);
            _acceptButton.gameObject.GetComponent<CanvasGroup>().alpha = 1;
            _numberInputField.text = "";
            _correctText.text = "";
            _numberInputField.interactable = true;
            this.gameObject.GetComponent<CanvasGroup>().alpha = 0;

            _numberInputField.GetComponent<TMP_InputField>().ActivateInputField();
            _numberInputField.GetComponent<TMP_InputField>().Select();
        }

        public void GetUserInput()
        {
            int userInput = int.Parse(_numberInputField.text);
            int correctNumber = _gameManager.CompareUserInput(userInput);
            if (userInput == correctNumber)
                _isCorrect = true;
            Hide(_acceptButton, 0.1F);
            OrganizeFrogs(correctNumber);
            _numberInputField.interactable = false;
            //UIAudio.Instance.PlayConfirmationClip();
        }

        public void ShowCorrect()
        {
            _correctText.GetComponent<CanvasGroup>().alpha = 0;
            LeanTween.alphaCanvas(_correctText.GetComponent<CanvasGroup>(), 1, 0.1F);
            if (_isCorrect)
            {
                _correctText.text = "¡Bien hecho!";
                _SFXManager.PlayAudio("Correct");
            }
            else
            {
                _correctText.text = "Vuelve a intentarlo";
                _SFXManager.PlayAudio("Wrong");
            }
            _isCorrect = false;
        }
        public void Hide(GameObject go, float time)
        {
            LeanTween.alphaCanvas(go.gameObject.GetComponent<CanvasGroup>(), 0, time);
            LeanTween.delayedCall(go, time, () => go.gameObject.SetActive(false));
        }
        public void OrganizeFrogs(int num)
        {
            List<GameObject> frogList = new List<GameObject>();
            for (int i = 0; i < num; i++)
            {
                GameObject go = Instantiate(_frogPrefab) as GameObject;
                if (i == 8)
                    go.transform.SetParent(_layoutA.transform, false);
                else if (i > 4)
                    go.transform.SetParent(_layoutB.transform, false);
                else
                    go.transform.SetParent(_layoutC.transform, false);

                go.gameObject.SetActive(false);

                frogList.Add(go);
            }
            StartCoroutine(CountFrogs(frogList, 0.25F));
        }
        public IEnumerator CountFrogs(List<GameObject> frogList, float waitTime)
        {
            foreach (var frog in frogList)
            {
                frog.gameObject.SetActive(true);
                yield return new WaitForSeconds(waitTime);
                _SFXManager.PlayAudio("Pop");
            }

            yield return new WaitForSeconds(waitTime * 3);
            ShowCorrect();

            yield return new WaitForSeconds(1.5F);
            Hide(this.gameObject, 0.1F);
            yield return new WaitForSeconds(0.05F);
            DestroyFrogs(frogList);
            _gameManager.StartCountdown();
            
        }

        public void DestroyFrogs(List<GameObject> frogList)
        {
            foreach (var frog in frogList)
            {
                Destroy(frog);
            }
        }
    }
}
