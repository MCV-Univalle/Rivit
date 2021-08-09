using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TMPro;

namespace SpaceShip
{
    public class TrysDisplayer : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        [SerializeField] private TextMeshProUGUI trysNumber;

        // Update is called once per frame
        void Update()
        {
            trysNumber.text = (_gameManager as SpaceShipGameManager).Attempts + "";
        }
    }
}