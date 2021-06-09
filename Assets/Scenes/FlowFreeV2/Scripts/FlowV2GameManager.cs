using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine;
using UnityEngine.UI;

namespace FlowFreeV2
{
    public class FlowV2GameManager : LevelSystemGameManager
    {
        public override string Name => "FlowFreeV2";
        [InjectOptional(Id = "SFXManager")] private AudioManager _SFXManager;

        private bool gameOver;

        public override void EndGame()
        {
           
        }

        public override void StartGame()
        {
            gameOver = false;
        }

        private void Update()
        {
            //DebugGUI._instance.flujosGUI = "Teimpo de juego = " + Time.realtimeSinceStartup;
            if (LineManager._instance.pathLineList.Count > 0)
            {
                gameOver = GameOver();

                DegugGUIUpdates();

                
                if (gameOver) GenerateBoard._instance.CreateBoardVoid();
                FlowFreeV2UIManager._instance.ActivePanelGameOver(gameOver);
            }
        }

        void DegugGUIUpdates()
        {
            DebugGUI._instance.flowCountGUI = "Flujos completos = " + LineManager._instance.CountFowCompleted();
            DebugGUI._instance.leveCompleted = "Nivel superado = " + GameOver();
        }

        public bool GameOver()
        {
            int cantFlowCompleted = LineManager._instance.CountFowCompleted();
            int cantFlowTotal = GenerateBoard._instance.CantLines;

            if (cantFlowCompleted == cantFlowTotal) return true;
            else return false;
        }

    }
}

