using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SpaceShip
{
    public class EnergyControlButton : MonoBehaviour
    {
        [SerializeField] private EnergyControl energyControl;
        [SerializeField] private TextMeshProUGUI numberIdText;


        public int Id {get; set;}
        public TextMeshProUGUI NumberIdText { get => numberIdText; set => numberIdText = value; }

        public void RegisterNumber()
        {
            int num = int.Parse(numberIdText.text);
            energyControl.ValidateInput(num);
        }
    }
}