using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SpaceShip
{
    public class OperationsEnergyControl : EnergyControl
    {
        private int answer;
        [SerializeField] private TextMeshProUGUI displayOperation;
        private List<int> possibleAnswers = new List<int>(4);
        private string MakeAddition()
        {
            int a = Random.Range(1, 10);
            int b = Random.Range(1, 10);
            answer = a + b;

            SetRandomNumbers(19);

            return System.String.Format("{0} + {1}", a, b);
        }

        private string MakeSustraction()
        {
            int a = Random.Range(1, 19);
            int b = Random.Range(1, 10);
            while(a < b)
            {
                b = Random.Range(1, 10);
            }

            answer = a - b;

            SetRandomNumbers(18);

            return System.String.Format("{0} - {1}", a, b);
        }

        private string MakeMultiplication()
        {
            int a = Random.Range(1, 10);
            int b = Random.Range(1, 10);
            answer = a * b;

            SetRandomNumbers(81);

            return System.String.Format("{0} x {1}", a, b);
        }

        private bool VerifyNotRepeatedNumber(int num)
        {
            bool isRepeated = false;
            foreach (int ans in possibleAnswers)
            {
                if (num == ans || num == answer)
                    isRepeated = true;
            }
            return isRepeated;
        }

        private void SetRandomNumbers(int max)
        {
            foreach (var button in buttonsArray)
            {
                int num = Random.Range(0, max);
                while(VerifyNotRepeatedNumber(num))
                {
                    num = Random.Range(0, max);
                }
                possibleAnswers.Add(num);
                button.NumberIdText.text = num + "";
            }
        }

        private void SetCorrectAnswer()
        {
            int temp = Random.Range(0, buttonsArray.Length);
            buttonsArray[temp].NumberIdText.text = answer + "";
        }

        public override void StartTask()
        {
            possibleAnswers = new List<int>(4);
            ChangeColor(colorLists[0]);
            string operation = "";
            int temp = Random.Range(1, 4);
            switch (temp)
            {
                case 1:
                    operation = MakeAddition();
                    break;
                case 2:
                    operation = MakeSustraction();
                    break;
                case 3:
                    operation = MakeMultiplication();
                    break;       
            }
            displayOperation.text = operation;
            SetCorrectAnswer();
        }

        public override void ValidateInput(int num)
        {
            if(num == answer)
                ShowPositiveFeedback();
            else
            {
                ShowNegativeFeedBack();
                StartCoroutine(RestartTask(0.75F));
            }
        }
    }
}