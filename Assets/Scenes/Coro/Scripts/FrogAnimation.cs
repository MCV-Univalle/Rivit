using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroMelodia
{
    public class FrogAnimation
    {
        private Animator _animator;
        private Animator _animatorReflect;

        public FrogAnimation(GameObject frog, GameObject frogReflect)
        {
            _animator = frog.gameObject.GetComponent<Animator>();
            _animatorReflect = frogReflect.gameObject.GetComponent<Animator>();
        }

        public void OpenFrog(bool value)
        {
            _animator.SetBool("open", value);
            _animatorReflect.SetBool("open", value);
        }
        public void BreathFrog()
        {
            _animator.SetTrigger("breath");
            _animatorReflect.SetTrigger("breath");
        }
        public void BlinkFrog()
        {
            _animator.SetTrigger("blink");
            _animatorReflect.SetTrigger("blink");
        }
        public void AppearFrog(bool value)
        {
            _animator.SetBool("visible", value);
            _animatorReflect.SetBool("visible", value);
        }
        public void SurpriseFrog(bool value)
        {
            _animator.SetBool("wrong", value);
            _animatorReflect.SetBool("wrong", value);
        }
    }
}