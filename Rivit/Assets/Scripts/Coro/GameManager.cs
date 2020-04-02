using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CoroMelodia
{
    public class GameManager : GameController
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                //Logic to create the instance
                if(_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    go.AddComponent<GameManager>();
                    _instance = go.GetComponent<GameManager>(); 
                }
                return _instance;
            }
        }

        public int NewNotes { get; set; } //the number of notes that will be added to the melody per iteration 
        public float Tempo { get; set; } //Time between each note (frog)
        //public bool RestartGame { get; set; }
        
        public bool IsRuningMelodyCoroutine { get; set; }
        public bool IsRuningPreparationCoroutine { get; set; }
        private IEnumerator _playMelodyCoroutine;
        private IEnumerator _preparationCoroutine;
        public IEnumerator PreparationCoroutine {get {return _preparationCoroutine;} set {_preparationCoroutine = value;}}

        void Awake()
        {
            _instance = this;
        }

        protected override void Start()
        {
            _uiManager = UIManager.Instance;
            base.Start();
            NewNotes = 0;
            IsRuningMelodyCoroutine = false;
            StartCoroutine(AnimationController.Instance.AppearEveryFrog(true));
        }

        public override void StartGame()
        {
            Fail = false;
            RestartGame = false;
            MelodyManager.Instance.EmptyMelody();
            LightingManager.Instance.TurnLightsOn();
            FrogsManager.Instance.AreFrogsBlocked = true;
            AdaptGameParameters();
            Tempo = 0.8F;
            Score = 0;
            AnimationController.Instance.ResetSurprise();
            AnimationController.Instance.ToDirectorFailPose(false);
            AnimationController.Instance.ToDirectorCorrectPose(false);
            IsGameStarted = true;

            _preparationCoroutine = ExtendMelody(3);

            StartCoroutine(_preparationCoroutine);
        }
        public override void FinishGame()
        {
            if(IsRuningPreparationCoroutine)
            {
                StopCoroutine(_preparationCoroutine);
                StartCoroutine(MessagesManager.Instance.ShowMessageBubble(false));
                LightingManager.Instance.TurnLightsOn();
                AnimationController.Instance.CloseEveryFrog();
            }
            if(IsRuningMelodyCoroutine)
            {
                StopCoroutine(_playMelodyCoroutine);
                StartCoroutine(MelodyManager.Instance.StopPlayingMelody());
            }
            IsGameStarted = false;
            IsRuningMelodyCoroutine = false;
            IsRuningPreparationCoroutine = false;
            MelodyManager.Instance.EmptyMelody();
        }

        public override void ShowResults()
        {
            base.ShowResults();
            UIManager.Instance.MainScreenIsActive = false;
            FrogsManager.Instance.AreFrogsBlocked = true;
        }

        public override void AdaptGameParameters()
        {
            //Adjus the game parametes according to the game mode
            
            if(GameMode == 0)
            {
                NewNotes = 1;
            }
            if(GameMode == 1)
            {
                NewNotes = 3;
            }
        }

        public bool EmptyMelody()
        {
            bool restartMelody = false;
            if(GameMode == 1)
            {
                NewNotes++;
                restartMelody = true;
            }
            return restartMelody;
        }

        public void CheckScore()
        {
            if(MelodyManager.Instance.MelodyNotes.Count == 3)
            {
                Tempo = 0.75F;
            }
            if(MelodyManager.Instance.MelodyNotes.Count == 5)
            {
                Tempo = 0.7F;
            }
        }

        public void ContinueMelody()
        {
            _preparationCoroutine = ExtendMelody(NewNotes);
            StartCoroutine(_preparationCoroutine);
        }

        public IEnumerator ExtendMelody(int notes)
        {
            IsRuningPreparationCoroutine = true;
            CheckScore();
            yield return new WaitForSeconds(1F);
            MessagesManager.Instance.ShowMessageBubble(false);
            AnimationController.Instance.ToDirectorCorrectPose(false);
            yield return new WaitForSeconds(0.5F);
            //Metronome sound
            MessagesManager.Instance.ShowPreparationMessage();
            bool restartMelody = EmptyMelody();
            MelodyManager.Instance.ExtendMelody(notes, restartMelody);
            StartCoroutine(UIManager.Instance.UpdateNumberMelodyNotes());
            for(int i = 0; i < 3; i++)
            {
                AudioManager.Instance.PlayMetronomeClack();
                yield return new WaitForSeconds(Tempo);
            }
            StartCoroutine(MessagesManager.Instance.ShowMessageBubble(false));
            AnimationController.Instance.ChangeDirectorPose();
            LightingManager.Instance.TurnLightsOff();
            yield return new WaitForSeconds(Tempo - 0.1F);
            _playMelodyCoroutine = MelodyManager.Instance.ShowMelody(Tempo);
            StartCoroutine(_playMelodyCoroutine);
            IsRuningPreparationCoroutine = false;
        }

        public IEnumerator PauseChant()
        {
            yield return new WaitForSeconds(0.4F);
            AudioManager.Instance.PauseChant();
        }
    }
}
