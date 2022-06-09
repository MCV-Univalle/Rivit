using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColorsFrogCounter
{
    public class ColorFrogsCounterGameManager : ModeSystemGameManager, IQuestionable
    {
        public int Rounds { get; set; }

        [SerializeField] private FrogsSpawner spawner;
        [SerializeField] private Countdown countdown;
        [SerializeField] private QuestionPanel questionPanel;

        [SerializeField] private TextMeshProUGUI questionText;
        [SerializeField] private Image sampleFrog;

        private int colorIndex;
        public override string Name => "Cuentaranas2";

        private IEnumerator coroutine;

        public override void IncreaseDifficulty()
        {
            _gameMode.IncreaseDifficulty(Rounds);
        }

        public override void EndGame()
        {
            spawner.StopSpawning();
            spawner.DestroyFrogs();
            StopCoroutine(coroutine);
        }

        public override void StartGame()
        {
            Rounds = 5;
            StartCountdown();
        }

        private void StartSpawning()
        {
            Rounds--;
            spawner.StartSpawning();
            float time = spawner.Total / 2F;
            coroutine = DestroyFrogs(time);
            StartCoroutine(coroutine);
        }

        private IEnumerator DestroyFrogs(float time)
        {
            yield return new WaitForSeconds(time);
            spawner.DestroyFrogs();
            OpenQuestionPanel();
        }

        public void OpenQuestionPanel()
        {
            questionPanel.gameObject.SetActive(true);
            colorIndex = Random.Range(0, spawner.MaxColors);
            ChangeQuestionText(colorIndex);
        }

        private void ChangeQuestionText(int num)
        {
            string colorName = "";
            Color color = spawner.ColorList[num];
            questionPanel.PrefabColor = spawner.ColorList[num];
            sampleFrog.color = color;
            switch(num)
            {
                case 0:
                    colorName = "verdes";
                    break;
                case 1:
                    colorName = "amarillas";
                    break;
                case 2:
                    colorName = "azules";
                    break;
                case 3:
                    colorName = "rojas";
                    break;
                case 4:
                    colorName = "púrpuras";
                    break;
            }
            questionText.text = string.Format("¿Cuántas ranas {0} habían?", colorName);
        }

        public int CompareUserInput(int userInput)
        {
            if (userInput == spawner.NumberOfFrogsForEachCholor[colorIndex])
                Score++;
            return spawner.NumberOfFrogsForEachCholor[colorIndex];
        }

        public void StartCountdown()
        {
            if (Rounds > 0)
                StartCoroutine(countdown.StartCountdown(3, 0.25F, () => StartSpawning()));
            else
                LeanTween.delayedCall(gameObject, 0.5F, () => NotifyGameOver());
        }
    }
}