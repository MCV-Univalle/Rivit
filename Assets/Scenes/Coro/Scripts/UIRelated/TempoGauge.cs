using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CoroMelodia
{
    public class TempoGauge : MonoBehaviour
    {
        [SerializeField] private GameObject fillCircle;
        [SerializeField] private GameObject musicIcon;
        [SerializeField] private List<Color> colorList;
        private bool _isGaugeActive = false;
        private float _tempo = 0.3F;

        private void Start()
        {
            gameObject.SetActive(false);
            Frog.singNote += Activate;
            Frog.stopSinging += Hide;
        }

        private void OnDestroy()
        {
            Frog.singNote -= Activate;
            Frog.stopSinging -= Hide;
        }

        private void Update()
        {
            IncrementGauge();
        }

        public void Activate(int num)
        {
            gameObject.SetActive(true);
            ChangeColor(num);
            _isGaugeActive = true;
            Show();
        }

        public void Show()
        {
            fillCircle.gameObject.GetComponent<Image>().fillAmount = 0;
            LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 1, 0.1F).setEaseInOutCubic();
        }
        public void Hide(int num)
        {
            _isGaugeActive = false;
            LeanTween.delayedCall(gameObject, 0.2F, () => gameObject.SetActive(false));
            LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 0, 0.1F).setEaseInOutCubic();
        }

        public void ChangeColor(int num)
        {
            musicIcon.GetComponent<Image>().color = colorList[num];
            fillCircle.GetComponent<Image>().color = colorList[num];
        }
        public void IncrementGauge()
        {
            if (_isGaugeActive)
            {
                fillCircle.gameObject.GetComponent<Image>().fillAmount += _tempo * Time.deltaTime;
            }
        }
    }
}
