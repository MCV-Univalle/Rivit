using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CoroMelodia
{
    public class MainScreenFader : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        private void Start()
        {
            Frog.singNote += Disable;
            Frog.stopSinging += Enable;
        }

        private void OnDestroy()
        {
            Frog.singNote -= Disable;
            Frog.stopSinging -= Enable;

        }

        public void Enable(int num)
        {
            if (!(_gameManager as MelodyChorusGameManager).IsGameStarted)
            {
                GetComponent<CanvasGroup>().alpha = 0;
                GetComponent<CanvasGroup>().interactable = false;
                StartCoroutine(WaitAndActive(2F));
            }
        }

        public IEnumerator WaitAndActive(float time)
        {
            yield return new WaitForSeconds(time);
            GetComponent<CanvasGroup>().interactable = true;
            LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 1, 0.1F)
                .setDelay(0.1F)
                .setEaseInOutCubic();
        }

        public void Disable(int num)
        {
            if (!(_gameManager as MelodyChorusGameManager).IsGameStarted)
            {
                StopAllCoroutines();
                LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 0, 0.1F)
                    .setDelay(0.01F)
                    .setEaseInOutCubic();
                GetComponent<CanvasGroup>().interactable = false;
            }

        }
    }
}