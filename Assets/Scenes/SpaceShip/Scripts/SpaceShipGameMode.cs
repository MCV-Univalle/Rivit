using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public abstract class SpaceShipGameMode : GameMode
    {
        [SerializeField] protected EnergyControl energyControlA;
        protected SpaceShipGameManager manager;
        private int nextTargetScore;
        protected int numberOfCorrects = 0;
        private IEnumerator currentCoroutine;

        public virtual void ContinueTaskAfterCharging()
        {
            numberOfCorrects++;
            currentCoroutine = ActiveTaskAgain();
            StartCoroutine(currentCoroutine);
        }

        public void AbortCoroutine()
        {
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);
        }
        protected virtual IEnumerator ActiveTaskAgain()
        {
            yield return new WaitForSeconds(3F);
            float waitTime = Random.Range(0, 5F);
            yield return new WaitForSeconds(waitTime);
            energyControlA.gameObject.SetActive(true);
            energyControlA.StartTask();
        }
        
        public override void IncreaseDifficulty(int score)
        {

            if (score > nextTargetScore)
            {
                if (nextTargetScore < 30)
                    nextTargetScore += 10;
                else if (nextTargetScore < 70)
                    nextTargetScore += 20;
                else if (nextTargetScore < 130)
                    nextTargetScore += 30;
                else if (nextTargetScore < 220)
                    nextTargetScore += 50;
                else if (nextTargetScore < 600)
                    nextTargetScore += 100;
                else if (nextTargetScore < 1000)
                    nextTargetScore += 1000;

                manager.AsteroidSpeed -= 0.15F;
                if (manager.MinSpawnSpeed > 0.1F)
                    manager.MinSpawnSpeed -= 0.075F;
                if (manager.MaxSpawnSpeed > 0.3F)
                    manager.MaxSpawnSpeed -= 0.095F;
                if (manager.EnergyGaugeDecreaseRate < 0.25F)
                    manager.EnergyGaugeDecreaseRate += 0.01F; manager.EnergyGaugeDecreaseRate += 0.01F;
            }
        }

        public override void InitializeSettings()
        {
            manager = _gameManager as SpaceShipGameManager;
            manager.EnergyControl = null;
            numberOfCorrects = 0;
            nextTargetScore = 10;
            manager.AsteroidSpeed = 4.5F;
            manager.MinSpawnSpeed = 1.2F;
            manager.MaxSpawnSpeed = 1.8F;
            manager.EnergyGaugeDecreaseRate = 0.04F;
            SetDefaultSettings();
        }

        public virtual void SetDefaultSettings()
        {
            manager.GameStarted = false;
            if (energyControlA != null)
            {
                LeanTween.delayedCall(this.gameObject, 0.5F, () => energyControlA.gameObject.SetActive(true));
                energyControlA.StartTask();
                manager.EnergyControl = energyControlA;
            }
        }
    }
}