using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class OrderedButtonsEnergyPanel : EnergyControl
    {
        private int _index = 0;
        private List<int> numberStack = new List<int>();
        private int[] correctOrder = new int[5];

        public override void ValidateInput(int num)
        {
            {
                if (num == _index)
                {
                    if (_index == 5)
                        ShowPositiveFeedback();
                    else
                    {
                        _index++;
                        _audioSource.clip = _sfxManager.GetAudio("Beep");
                        _audioSource.Play();
                    }
                }
                else
                {
                    ShowNegativeFeedBack();
                    StartCoroutine(RestartTask(0.75F));
                }
            }
        }

        private void GenerateOrder()
        {
            ChangeColor(colorLists[0]);
            numberStack = new List<int>();
            var tempList = new List<int> { 1, 2, 3, 4, 5 };
            for (int i = 0; i < correctOrder.Length; i++)
            {
                int num = Random.Range(0, tempList.Count);
                buttonsArray[i].NumberIdText.text = tempList[num] + "";
                tempList.RemoveAt(num);
            }
        }

        public override void StartTask()
        {
            _index = 1;
            GenerateOrder();
        }
    }
}