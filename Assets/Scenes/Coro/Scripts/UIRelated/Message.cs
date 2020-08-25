using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CoroMelodia
{
    public class Message : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private GameObject _pressFrogIndication;

        private void Start()
        {
            GameManager.endGame += Hide;
        }

        private void OnDestroy()
        {
            GameManager.endGame -= Hide;
        }

        public void Display(string message)
        {
            gameObject.SetActive(true);
            LeanTween.scale(gameObject.GetComponent<RectTransform>(), new Vector3(1, 1, 1), 0.075F);
            LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 1, 0.1F)
                .setEaseInOutCubic();
            messageText.text = message;
        }

        public void Hide()
        {
            LeanTween.delayedCall(gameObject, 0.21F, () => gameObject.SetActive(false));
            LeanTween.scale(gameObject.GetComponent<RectTransform>(), new Vector3(0.2F, 0.2F, 0.2F), 0.075F)
                .setDelay(0.1F);
            LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 0, 0.1F)
                .setDelay(0.1F)
                .setEaseInOutCubic();
        }
    }
}