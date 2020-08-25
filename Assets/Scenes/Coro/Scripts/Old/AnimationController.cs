using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
namespace CoroMelodia
{
    public class AnimationController : MonoBehaviour
    {
        private static AnimationController _instance;
        public static AnimationController Instance
        {
            get
            {
                //Logic to create the instance
                if(_instance == null)
                {
                    GameObject go = new GameObject("AnimationController");
                    go.AddComponent<AnimationController>();
                    _instance = go.GetComponent<AnimationController>(); 
                }
                return _instance;
            }
        }

        private Animator[] _frogsAnimatorList;
        private Animator[] _frogsReflectAnimatorList;
        private Animator _directorFrogAnim;
        private int _directorPose;

        void Awake()
        {
            _instance = this;
            _directorFrogAnim = FrogsManager.Instance.DirectorFrog.GetComponent<Animator>();
            _directorPose = 0;
            _frogsAnimatorList = new Animator[6];
            _frogsReflectAnimatorList = new Animator[6];
            //Get the animator component for every frog
            //Assumptions: there are only 6 frogs and the director
            GameObject[] frogList = FrogsManager.Instance.FrogList;
            for(int i = 0; i < 6; i++)
            {
                _frogsAnimatorList[i] = frogList[i].GetComponent<Animator>();
                _frogsReflectAnimatorList[i] = frogList[i].gameObject.transform.GetChild(0).gameObject.GetComponent<Animator>();
            }
        }

        //Director

        public void BlinkDirector()
        {
            _directorFrogAnim.SetTrigger("blink");
        }
        public void BreathDirector()
        {
            _directorFrogAnim.SetTrigger("breath");
        }
        public void ToDirectorCorrectPose(bool value)
        {
            _directorFrogAnim.SetBool("correct", value);
        }
        public void ToDirectorFailPose(bool value)
        {
            _directorFrogAnim.SetBool("fail", value);
        }
        public void ToDirectorIdle()
        {
            _directorFrogAnim.SetTrigger("idle");
            _directorPose = 0;
        }
        public void ChangeDirectorPose()
        {
            if(_directorPose == 6 )
            {
                _directorPose = 2;
            }
            string poseName = "pose" + _directorPose;
            _directorFrogAnim.SetTrigger(poseName);
            _directorPose++;
        }


        //Frogs

        public void OpenFrog(int numFrog, bool value)
        {
            _frogsAnimatorList[numFrog].SetBool("open", value);
            _frogsReflectAnimatorList[numFrog].SetBool("open", value);
            if(value)
            {
                AudioManager.Instance.PlayChant(numFrog);
            }
            else
            {
                AudioManager.Instance.FadeOut(0.12F);
            }
        }
        public void BreathFrog(int numFrog)
        {
            _frogsAnimatorList[numFrog].SetTrigger("breath");
            _frogsReflectAnimatorList[numFrog].SetTrigger("breath");
        }
        public void BlinkFrog(int numFrog)
        {
            _frogsAnimatorList[numFrog].SetTrigger("blink");
            _frogsReflectAnimatorList[numFrog].SetTrigger("blink");
        }
        public void AppearFrog(int numFrog, bool value)
        {
            _frogsAnimatorList[numFrog].SetBool("visible", value);
            _frogsReflectAnimatorList[numFrog].SetBool("visible", value);
        }
        public void SurpriseFrog(int numFrog, bool value)
        {
            _frogsAnimatorList[numFrog].SetBool("wrong", value);
            _frogsReflectAnimatorList[numFrog].SetBool("wrong", value);
        }

        public IEnumerator AppearEveryFrog(bool value)
        {
            //Assumptions: there are only 6 frogs
            float intervalTime = 0.09F;
            yield return new WaitForSeconds(intervalTime * 2);
            for(int i = 0; i < 6; i++)
            {
                AudioManager.Instance.PlayBounce();
                AppearFrog(i,true);
                yield return new WaitForSeconds(intervalTime);
            }
        }

        public void ResetSurprise()
        {
            //Make false the bool of "wrong" for every frog
            //Assumptions: there are only 6 frogs
            for(int i = 0; i < 6; i++)
            {
                SurpriseFrog(i, false);
            }
        }

        public void CloseEveryFrog()
        {
            //Make the "open" trigger frog for every frog so it will not remain any frog with the mouth opened
            for(int i = 0; i < 6; i++)
            {
                _frogsAnimatorList[i].SetBool("open", false);
                _frogsReflectAnimatorList[i].SetBool("open", false);
            }   
        }
    }
}
*/