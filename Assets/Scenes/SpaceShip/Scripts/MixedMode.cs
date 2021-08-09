using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class MixedMode : SpaceShipGameMode
    {
        [SerializeField] private EnergyControl energyControlB;

        protected override IEnumerator ActiveTaskAgain()
        {
            int num = Random.Range(0, 2);
            manager.EnergyControl = num == 0 ? this.energyControlA : this.energyControlB;

            yield return new WaitForSeconds(3F);
            float waitTime = Random.Range(0, 5F);
            yield return new WaitForSeconds(waitTime);
            manager.EnergyControl.gameObject.SetActive(true);
            //manager.EnergyControl.StartTask();
        }

        public override void SetDefaultSettings()
        {
            manager.GameStarted = true;
            manager.EnergyGaugeDecreaseRate = 0.04F;
            manager.RunEverything();
            StartCoroutine(ActiveTaskAgain());
        }
    }
}