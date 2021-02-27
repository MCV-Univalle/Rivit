using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;
using System;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace Home
{
    public delegate void Notify();
    public class UIManager : MonoBehaviour
    {

        public bool IsBlocked { get; set; }
        [SerializeField]
        private WhiteScreen whiteScreen;
        [SerializeField]
        private GameObject _blackPanel;
        public static event Notify gameListButtonEvent;
        public static event Notify closeGameListButtonEvent;

        void Awake()
        {
            IsBlocked = false;
        }

        private void Start()
        {
            whiteScreen.FadeAlpha(1, 0, 0.35F, 1.25F);
        }

        public IEnumerator BlockUI()
        {
            IsBlocked = true;
            yield return new WaitForSeconds(0.25F);
            IsBlocked = false;
        }

        public void InvokeGameListButtonEvent()
        {
            gameListButtonEvent();
        }

        public void InvokeCloseGameListButtonEvent()
        {
            closeGameListButtonEvent();
        }

        public void SwitchScene(string sceneName)
        {
            whiteScreen.FadeAlpha(0, 1F, 0.25F, 0F);
            LeanTween.delayedCall(gameObject, 0.9F, () => SceneManager.LoadScene(sceneName));
        }

        private void OnDestroy()
        {
            UnsuscribeListeners();
        }
        public void UnsuscribeListeners()
        {
            gameListButtonEvent = null;
            closeGameListButtonEvent = null;
        }
    }   
}
