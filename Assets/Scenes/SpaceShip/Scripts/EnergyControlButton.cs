using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceShip
{
    public class EnergyControlButton : MonoBehaviour
    {
        [SerializeField] private EnergyControl energyControl;
        [SerializeField] private Text numberIdText;

        public bool IsCorrect { get; set; }

        public int Id {get; set;}
        public Text NumberIdText { get => numberIdText; set => numberIdText = value; }

        public void RegisterNumber()
        {
            int num = int.Parse(numberIdText.text);
            if(energyControl.ValidateInput(num))
            {
                IsCorrect = true;
            }
            else
            {

            }
        }
    }
}