using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
namespace CoroMelodia
{
    public class FrogsClickEvent : MonoBehaviour
    {
        [SerializeField]
        private int _numbFrog; //The correspondent number of the frog
        private bool _isClickAvaible = true;
        private bool _frogsBlock = false;
        [SerializeField]
        private AudioClip _chantVoice;
        public AudioClip ChantVoice { get { return _chantVoice; } }
        // Start is called before the first frame update
        void Start()
        {
            
        }

        void OnMouseDown()
        {
            _frogsBlock = FrogsManager.Instance.AreFrogsBlocked; 
            if(!_frogsBlock)
            {
                if(_isClickAvaible)
                {
                    FrogsManager.Instance.PressFrog(_numbFrog);
                    _isClickAvaible = !_isClickAvaible;
                }
            }
        }

        void OnMouseUp()
        {
            if(!_frogsBlock)
            {
                FrogsManager.Instance.ReleaseFrog(_numbFrog);
                _isClickAvaible = !_isClickAvaible;
            }
        }
    }   
}
*/