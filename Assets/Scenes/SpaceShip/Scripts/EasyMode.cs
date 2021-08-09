using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class EasyMode : SpaceShipGameMode
    {
        [SerializeField] private GameObject hand;
        
        public override void SetDefaultSettings()
        {
            StartCoroutine(StartPreparations());
        }
        private IEnumerator StartPreparations()
        {
            manager.ShowIndicativeText("Desliza tu dedo para desplazar la nave", 4F);
            hand.SetActive(true);
            yield return new WaitForSeconds(4F);
            manager.RunEverything();
            hand.SetActive(false);
        }
    }
}