using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Home
{
   public class AnimationManager : MonoBehaviour
    {
        private static AnimationManager _instance;
        public static AnimationManager Instance
        {
            get
            {
                //Logic to create the instance
                if(_instance == null)
                {
                    GameObject go = new GameObject("AnimationManager");
                    go.AddComponent<AnimationManager>();
                    _instance = go.GetComponent<AnimationManager>();
                }
                return _instance;
            }
        }
        [SerializeField]
        private GameObject _eyes;
        private Animator _eyesAnimator;
        [SerializeField]
        private GameObject _mouth;
        private Animator _mouthAnimator;

        private bool _isActive = true;

        void Awake()
        {
            _instance = this;
            _eyesAnimator = _eyes.gameObject.GetComponent<Animator>();
            _mouthAnimator = _mouth.gameObject.GetComponent<Animator>();
        }

        void Start()
        {
            StartCoroutine(Blink());
        }

        public void TriggerBlink()
        {
            _eyesAnimator.SetTrigger("blink");
        }

        public IEnumerator Blink()
        {
            while(_isActive)
            {
                yield return new WaitForSeconds(2.5F);
                int randomNum = Random.Range(0, 2);
                if(randomNum == 0) TriggerBlink();
            }
        }

        public void ShowMouthSpeaking(bool value)
        {
            _mouthAnimator.SetBool("active", value);
        }



    } 
}
