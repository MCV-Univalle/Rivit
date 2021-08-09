using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

namespace SpaceShip
{
    public class SpaceShipGameManager : ModeSystemGameManager
    {
        [SerializeField] private AsteroidManager asteroidManager;
        [SerializeField] private EnergyControl energyControl;
        [SerializeField] private Ship ship;
        [SerializeField] private TextMeshProUGUI indicativeText; 

        private IEnumerator spawnLoop;

        public bool GameStarted { get; set; }

        private SpaceShipAdditionalData additionalData;
        public float TotalTime { get; set; }


        private int attempts = 3;

        [Inject(Id = "SFXManager")] AudioManager SFXManager;

        public override string Name => "SpaceShip";

        public int Attempts { get => attempts; set => attempts = value; }

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

        public float EnergyGaugeDecreaseRate { get; set; }

        public bool IsGaugeCharged
        {
            get => energyControl.IsGaugeCharged;
        }
        public EnergyControl EnergyControl { get => energyControl; set => energyControl = value; }
        public SpaceShipAdditionalData AdditionalData { get => additionalData; set => additionalData = value; }

        private void Start()
        {
            EnergyControl.energyRunOut += EnergyRunOut;
        }

        private void EnergyRunOut()
        {
            ShowIndicativeText("Se agoto la energia", 2.5F);
            ReduceAttempts();
        }

        private void Update()
        {
            if(ship.State == ShipState.Dead)
            {
                ReduceAttempts();
            }
            if (ship.HasBonus)
            {
                SFXManager.PlayAudio("Sparkle");
                ship.HasBonus = false;
                Score += 3;
            }
        }

        private void ReduceAttempts()
        {
            Attempts--;
            if (Attempts <= 0)
                GameOver();
            else
            {
                EndGame();
                LeanTween.delayedCall(this.gameObject, 3F, () => RunEverything());
            }
        }

        private void GameOver()
        { 
            EndGame();
            LeanTween.delayedCall(gameObject, 2.5F, () => NotifyGameOver());
        }

        public override void EndGame()
        {
            additionalData.AverageTime = TotalTime / (additionalData.CorrectAnswers + additionalData.WrongAnswers);
            if (energyControl != null) energyControl.gameObject.SetActive(false);
            ship.State = ShipState.Inactive;
            if(spawnLoop != null) StopCoroutine(spawnLoop);
            RaiseEndGameEvent();
            (_gameMode as SpaceShipGameMode).AbortCoroutine();
        }

        public override void StartGame()
        {
            attempts = 3;
            additionalData = new SpaceShipAdditionalData();
        }

        public override string RegisterAdditionalData()
        {
            return JsonConvert.SerializeObject(additionalData);
        }

        public void ContinueWithTask()
        {
            if(!GameStarted)
            {
                GameStarted = true;
                RunEverything();
                energyControl.ChargeEnergy();
            }
            else
                if (_gameMode != null) (_gameMode as SpaceShipGameMode).ContinueTaskAfterCharging();
        }

        public void RunEverything()
        {
            if(GameStarted)
            {
                SetShipToDefault();
                spawnLoop = asteroidManager.StartSpawningCycle();
                ShowIndicativeText("¡Preparate!", 3F);
                LeanTween.delayedCall(this.gameObject, 2.5F, () => StartCoroutine(spawnLoop));
                LeanTween.delayedCall(this.gameObject, 2.5F, () => ActivateTasks());
            }
            ActivateTasks();
        }

        private void ActivateTasks()
        {
            if (energyControl != null)
            {
                energyControl.gameObject.SetActive(true);
            }
        }

        private void SetShipToDefault()
        {
            ship.gameObject.SetActive(true);
            ship.gameObject.transform.position = new Vector3(0, -1.5F, 0);
            ship.State = ShipState.Idle;
        }

        public void ShowIndicativeText(string text, float time)
        {
            indicativeText.text = text;
            indicativeText.gameObject.SetActive(true);
            LeanTween.delayedCall(this.gameObject, time, () => indicativeText.gameObject.SetActive(false));
        }

        public void ShowIndicativeText(string text)
        {
            indicativeText.text = text;
            indicativeText.gameObject.SetActive(true);
        }
    }
}