using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroMelodia
{
    public class BreathAndBlinkScript
    {
        private bool _isActive = true;
        public bool IsActive 
        { 
            get 
            { 
                return _isActive;
            } 
            set 
            {
                _isActive = value;
            } 
        }

        void Awake()
        {
            IsActive = true;
        }

        public IEnumerator BlinkFrog()
        {
            int cont = 0;
            while(_isActive)
            {
                if(cont == 5)
                {
                    cont = 0;
                    yield return new WaitForSeconds(2F);    
                }
                yield return new WaitForSeconds(1F);
                int randomNum = Random.Range(0, 9);
                if(randomNum <= 5)
                {
                    AnimationController.Instance.BlinkFrog(randomNum);
                    cont++;
                }
                else if (randomNum == 6)
                {
                    AnimationController.Instance.BlinkDirector();
                }
                else 
                {
                    cont--;
                }
            }
        }
        public IEnumerator BreathFrog()
        {
            int cont = 0;
            while(_isActive)
            {
                if(cont == 4)
                {
                    cont = 0;
                    yield return new WaitForSeconds(2F);    
                }
                yield return new WaitForSeconds(1F);
                int randomNum = Random.Range(0, 9);
                if(randomNum <= 5)
                {
                    AnimationController.Instance.BreathFrog(randomNum);
                    cont++;
                }
                else if (randomNum == 6)
                {
                    AnimationController.Instance.BreathDirector();
                }
                else 
                {
                    cont--;
                }
            }
        }
    } 
}

