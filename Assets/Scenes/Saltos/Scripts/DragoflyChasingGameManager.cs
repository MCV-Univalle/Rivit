using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace saltos
{
   public class DragoflyChasingGameManager : GameManager
    {
        public override string Name => "DragoflyChasing";
        //[Inject(Id = "SFXManager")] private AudioManager _SFXManager;

        private void Start()
        {
            Debug.Log("Srart");
        }

        public override void StartGame()
        {
            Debug.Log("SrartGame");
        }


        public override void EndGame()
        {
            Debug.Log("EndGame");
        }
    }
}

