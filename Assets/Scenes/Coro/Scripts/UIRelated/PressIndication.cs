using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CoroMelodia
{
    public class PressIndication : MonoBehaviour
    {
        [SerializeField] private GameObject handIcon;
        [Inject] private GameManager _gameManager;
        private void Start()
        {
            Frog.singNote += Disable;
            Frog.stopSinging += Enable;
            UIManager.executePlayButton += Desactivar;
            UIManager.executeHelpButton += Desactivar;
            UIManager.executeCloseModeSelectionButton += Activar;
            UIManager.executeCloseInstructions += Activar;
            Enable(0);
            TweenHand();
        }

        private void OnDestroy()
        {
            Frog.singNote -= Disable;
            Frog.stopSinging -= Enable;
            UIManager.executePlayButton -= Desactivar;
            UIManager.executeHelpButton -= Desactivar;
            UIManager.executeCloseModeSelectionButton -= Activar;
            UIManager.executeCloseInstructions -= Activar;
        }

        public void Activar()
        {
            Enable(0);
        }
        public void Desactivar()
        {
            Disable(0);
        }

        public void Enable(int num)
        {
            if (!(_gameManager as MelodyChorusGameManager).IsGameStarted)
            {
                gameObject.SetActive(true);
                GetComponent<CanvasGroup>().alpha = 0;
                StartCoroutine(WaitAndActive(4.5F));
            }

        }

        public void TweenHand()
        {
            LeanTween.moveY(handIcon.GetComponent<RectTransform>(), 32F, 0.75F).setEaseInOutCubic().setLoopPingPong();
        }

        public IEnumerator WaitAndActive(float time)
        {
            yield return new WaitForSeconds(time);
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
                    .setDelay(0.1F)
                    .setEaseInOutCubic();
                LeanTween.delayedCall(gameObject, 0.3F, () => gameObject.SetActive(false));
            }

        }
    }
}