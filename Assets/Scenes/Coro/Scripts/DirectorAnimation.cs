using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroMelodia
{
    public class DirectorAnimation
    {
        private Animator _animator;
        private int _pose = 1;

        public DirectorAnimation(Animator animator)
        {
            _animator = animator;
        }

        public void Blink()
        {
            _animator.SetTrigger("blink");
        }
        public void Breath()
        {
            _animator.SetTrigger("breath");
        }
        public void ToCelebrationPose(bool value)
        {
            _animator.SetBool("correct", value);
        }
        public void ToFailPose(bool value)
        {
            _animator.SetBool("fail", value);
        }
        public void ToPreparation()
        {
            _animator.SetBool("idle", false);
            _animator.SetTrigger("preparation");
        }
        public void ToIdle()
        {
            _animator.SetBool("idle", true);
            _pose = 1;
        }
        public void ChangePose()
        {
            if(_pose == 6 )
            {
                _pose = 2;
            }
            string poseName = "pose" + _pose;

            _animator.SetTrigger(poseName);
            _pose++;
        }
    }
}