using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CoroMelodia
{
    public class MelodyChorusGameManager : ModeSystemGameManager
    {
        public override string Name => "MelodyChorus";
        public int MelodyLong => _melody.Notes.Count;

        public bool IsGameStarted { get => _isGameStarted; set => _isGameStarted = value; }
        public float Speed { get => _melodyPlayer.Speed; set => _melodyPlayer.Speed = value; }

        [SerializeField] private Melody _melody;
        [SerializeField] private MelodyPlayer _melodyPlayer;
        [SerializeField] private FrogsManager frogsManager;
        [Inject(Id = "SFXManager")] private AudioManager _SFXManager;

        private bool _isGameStarted = false;
        private bool _gameOver = false;

        private void Start()
        {
            Frog.singNote += CatchNote;
            Frog.stopSinging += VerifyIfMelodyIsFinished;
            Frog.stopSinging += FinishGame;
        }
        private void Update()
        {
            PlayMelody();
        }

        private void OnDestroy()
        {
            Frog.singNote -= CatchNote;
            Frog.stopSinging -= VerifyIfMelodyIsFinished;
            Frog.stopSinging -= FinishGame;
        }

        public void VerifyIfMelodyIsFinished(int num)
        {
            if (_melody.IsFinished)
            {
                frogsManager.BlockFrogs();
                VerifyIfMelodyReachedLimit();
            }
        }

        public void VerifyIfMelodyReachedLimit()
        {
            if (MelodyLong >= (_gameMode as MelodyChorusGameMode).Limit)
                LeanTween.delayedCall(gameObject, 0.5F, () => NotifyGameOver());
            else
            {
                string message = (_gameMode as MelodyChorusGameMode).IncrementMelody();
                _melody.IsFinished = false;
                LeanTween.delayedCall(gameObject, 1F, () => PrepareToShowMelody(message));
            }
        }
        public void PlayMelody()
        {
            if (_melodyPlayer.IsReadyToStartMelody)
            {
                StartCoroutine(_melodyPlayer.PlayMelody());
                _melodyPlayer.IsReadyToStartMelody = false;
            }
        }

        public void StartNewMelody(int num)
        {
            _melody.EmptyMelody();
            _melody.AddNotes(num);
        }

        public void PrepareToShowMelody(string message)
        {
            frogsManager.ToDefault();
            StartCoroutine(_melodyPlayer.PrepareToStartMelody(message));
            IsGameStarted = true;
        }
        public override void StartGame()
        {
            StartNewMelody(3);
            PrepareToShowMelody("Memoriza esta melodía");
        }
        public void CatchNote(int note)
        {
            if (IsGameStarted)
            {
                int correctNote = _melody.CompareWithCorrectNote(note);
                if (correctNote == -1)
                {
                    Score++;
                    Celebrate();
                }
                else
                    ShowCorrectFrog(correctNote);

            }
        }

        public void ShowCorrectFrog(int correctNote)
        {
            _SFXManager.PlayAudio("Wrong");
            frogsManager.ShowCorrect(correctNote);
            _gameOver = true;
        }

        public void FinishGame(int num)
        {
            if (_gameOver)
            {
                _gameOver = false;
                if (Score == 0)
                    PrepareToShowMelody("Íntentalo otra vez");
                else
                    LeanTween.delayedCall(gameObject, 0.5F, () => NotifyGameOver());
            }
        }

        public void Celebrate()
        {
            if (_melody.IsFinished)
            {
                frogsManager.ToCelebrationPose();
                _SFXManager.PlayAudio("Correct");
            }
        }

        public void DisplayMessage(string message)
        {
            frogsManager.DisplayMessage(message);
        }
        public override void EndGame()
        {

            StopAllCoroutines();
            IsGameStarted = false;
            RaiseEndGameEvent();
        }

        public void EmptyMelody()
        {
            _melody.EmptyMelody();
        }
        public void AddNote()
        {
            _melody.AddNote();
        }
    }
}