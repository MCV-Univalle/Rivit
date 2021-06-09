using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SpaceShip
{
    public class SpaceShipGameManager : ModeSystemGameManager
    {
        [SerializeField] private AsteroidManager asteroidManager;
        [SerializeField] private EnergyControl energyControl;
        [SerializeField] private Ship ship;

        [Inject(Id = "SFXManager")] AudioManager SFXManager;

        private IEnumerator spawnLoop;

        public float AsteroidSpeed 
        {
            get => asteroidManager.AsteroidSpeed;
            set { asteroidManager.AsteroidSpeed = value; } 
        }
        public float MinSpawnSpeed
        {
            get => asteroidManager.MinSpawnSpeed;
            set { asteroidManager.MinSpawnSpeed = value; }
        }
        public float MaxSpawnSpeed
        {
            get => asteroidManager.MaxSpawnSpeed;
            set { asteroidManager.MaxSpawnSpeed = value; }
        }

        public float EnergyGaugeDecreaseRate
        {
            get => energyControl.EnergyDecreaseRate;
            set { energyControl.EnergyDecreaseRate = value; }
        }
        public override string Name => "SpaceShip";

        private void Start()
        {
            EnergyControl.energyRunOut += GameOver;
        }

        private void Update()
        {
            if(ship.State == ShipState.Dead)
            {
                GameOver();
            }
            if (ship.HasBonus)
            {
                SFXManager.PlayAudio("Sparkle");
                ship.HasBonus = false;
                Score += 3;
            }
        }

        private void GameOver()
        { 
            EndGame();
            LeanTween.delayedCall(gameObject, 0.5F, () => NotifyGameOver());
        }

        public override void EndGame()
        {
            ship.State = ShipState.Inactive;
            RaiseEndGameEvent();
            StopCoroutine(spawnLoop);
            energyControl.gameObject.SetActive(false);
        }

        public override void StartGame()
        {
            ship.gameObject.SetActive(true);
            LeanTween.delayedCall(this.gameObject, 0.5F, () => energyControl.gameObject.SetActive(true));
            ship.gameObject.transform.position = new Vector3(0, -1.5F, 0);
            ship.State = ShipState.Idle;
            spawnLoop = asteroidManager.StartSpawningCycle();
            StartCoroutine(spawnLoop);
        }
    }
}