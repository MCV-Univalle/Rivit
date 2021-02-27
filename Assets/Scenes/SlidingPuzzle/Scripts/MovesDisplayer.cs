using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;

namespace SlidingPuzzle
{
    public class MovesDisplayer : MonoBehaviour
    {
        SlidingPuzzleGameManager _gameManager;
        [SerializeField] TextMeshProUGUI moveText;

        [Inject]
        public void Construct(GameManager gameManager)
        {
            this._gameManager = gameManager as SlidingPuzzleGameManager;
        }
        void Update()
        {
            moveText.text = _gameManager.Moves + "";
        }
    }
}