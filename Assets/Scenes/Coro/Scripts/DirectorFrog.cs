using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroMelodia
{
    public class DirectorFrog : MonoBehaviour
    {
        private DirectorAnimation _directorAnimation;
        [SerializeField] private Message messageBubble;

        private void Start()
        {
            GameManager.endGame += ToDefault;
            _directorAnimation = new DirectorAnimation(gameObject.GetComponent<Animator>());
            StartCoroutine(Breath());
            StartCoroutine(Blink());
        }

        private void OnDestroy()
        {
            GameManager.endGame -= ToDefault;
        }

        public void ToDefault()
        {
            _directorAnimation.ToIdle();
            _directorAnimation.ToFailPose(false);
            _directorAnimation.ToCelebrationPose(false);
        }

        public void DisplayMessage(string text)
        {
            messageBubble.Display(text);
        }

        public void HideMessage()
        {
            messageBubble.Hide();
        }

        public void Prepare()
        {
            _directorAnimation.ToPreparation();
        }

        public void ChangePose()
        {
            _directorAnimation.ChangePose();
        }

        public void ToFailPose()
        {
            _directorAnimation.ToFailPose(true);
        }
        public void ToCelebrationPose()
        {
            _directorAnimation.ToCelebrationPose(true);
        }

        public IEnumerator Blink()
        {
            while (gameObject.activeInHierarchy)
            {
                int randomNum = Random.Range(0, 15);
                if (randomNum == 5)
                    _directorAnimation.Blink();
                yield return new WaitForSeconds(0.5F);
            }
        }

        public IEnumerator Breath()
        {
            while (gameObject.activeInHierarchy)
            {
                int randomNum = Random.Range(0, 15);
                if (randomNum == 5)
                    _directorAnimation.Breath();
                yield return new WaitForSeconds(0.75F);
            }
        }
    }
}
