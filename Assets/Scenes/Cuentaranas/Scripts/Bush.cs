using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace Cuentaranas
{
    public class Bush : MonoBehaviour
    {
        [Inject (Id="SFXManager")] private AudioManager _SFXManager;

        private void Start()
        {
            GameManager.endGame += ToOriginalPosition;
        }
        private void OnDestroy()
        {
            GameManager.endGame -= ToOriginalPosition;
        }

        public void ToOriginalPosition()
        {
            gameObject.transform.localPosition = new Vector3(0, 200, 0);
        }

        public void Fall()
        {
            ToOriginalPosition();
            _SFXManager.PlayAudio("Falling");
            LeanTween.moveY(gameObject.GetComponent<RectTransform>(), 0, 0.5F)
            .setEaseInOutCubic();
        }
    }
}
