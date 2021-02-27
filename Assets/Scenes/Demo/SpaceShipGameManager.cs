using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class SpaceShipGameManager : GameManager
    {
        public int score = 0;
        public override string Name => "SpaceShip";

        public override void EndGame()
        {
            throw new System.NotImplementedException();
        }

        public override void StartGame()
        {
            throw new System.NotImplementedException();
        }
    }
}