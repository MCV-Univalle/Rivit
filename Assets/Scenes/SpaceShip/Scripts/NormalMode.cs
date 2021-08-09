using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class NormalMode : SpaceShipGameMode
    {
        public override void SetDefaultSettings()
        {
            base.SetDefaultSettings();
            manager.ShowIndicativeText("Presiona los botones en orden creciente");
        }
    }
}