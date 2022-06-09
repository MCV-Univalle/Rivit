using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroMelodia
{
    public class FreeMode : MelodyChorusGameMode
    {
        [SerializeField] private GameObject inGameScreen;
        [SerializeField] private GameObject returnButton;
        public override void IncreaseDifficulty(int score)
        {

        }

        public override string IncrementMelody()
        {
            throw new System.NotImplementedException();
        }

        public override void InitializeSettings()
        {
            var gameManager = (MelodyChorusGameManager)_gameManager;
            inGameScreen.SetActive(false);
            returnButton.gameObject.SetActive(true);
            gameManager.FreeMode = true;
        }

        public void Return()
        {
            returnButton.gameObject.SetActive(false);
        }
    }
}