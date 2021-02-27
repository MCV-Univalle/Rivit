using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Tuberias
{
    public class TuberiasGameManager : GameManager
    {
        public override string Name => "Tuberias";
        [InjectOptional(Id = "SFXManager")] private AudioManager _SFXManager;

        public override void EndGame()
        {
            Debug.Log("EndGameEnter");
        }

        public override void StartGame()
        {
            Debug.Log("StartGameEnter");
            Debug.Log("Metodo Start: FlowFree");

        }

        private void Start()
        {
            Debug.Log("Metodo Start: FlowFree");
        }

        private void Update()
        {

        }

    }
}

