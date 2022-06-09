using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;
using Tuberias;
using Newtonsoft.Json;

namespace Tuberias
{
    public class TuberiasGameManager : LevelSystemGameManager
    {
        public override string Name => "Tuberias";
        [InjectOptional(Id = "SFXManager")] private AudioManager SFXManager;
        [SerializeField] Timer timer;
        private bool gameOver;
        public int validar;
        public int tamaño;
        public int levelIndexList;
        public List<int> listaVer;
        public int contadorMovimientos;
        public GameObject[] listaObjetos;
        public List<TextAsset> levelsList2;
        private GameObject boton;
        public int clicks;

        private TuberiasAdditionalData additionalData;

        public override void EndGame()
        {
            Debug.Log("EndGameEnter");
            validar=0;
            tamaño=2;
            TuberiasUIManager.instance.ActivePanelControlsTuberias(false);
        }

        public override string RegisterAdditionalData()
        {
            additionalData.Moves = this.contadorMovimientos;
            additionalData.Time = timer.CurrentTime;
            return JsonConvert.SerializeObject(additionalData);
        }

        public override void StartGame()
        {
            additionalData = new TuberiasAdditionalData();
            Debug.Log("Metodo Start: Tuberias");
            gameOver = false;
            levelIndexList = 0;
            timer.IsIncrementing = true;
            //TuberiasUIManager.instance.ActivePanelGameOver(gameOver);
            //TuberiasUIManager.instance.DesactiveBoard(!gameOver);
        }

        private void Update()
        {
            Verificacion();

            if(gameOver)
            {
            manejadorTablero.instance.CrearTableroVacio();
            //TuberiasUIManager.instance.DesactiveBoard(!gameOver);
            TuberiasUIManager.instance.ActivePanelGameOver(gameOver);
            manejadorTablero.instance.angulos.Clear();
            timer.Started = false;
            gameOver=false;
            }
        }

        void DegugGUIUpdates()
        {
            DebugGUITuberias.instance.movimientos = "Movimientos = " + clicks;
            DebugGUITuberias.instance.levelCompleted = "Nivel superado = " + gameOver;
        }


        private void Verificacion()
        {
            listaObjetos = GameObject.FindGameObjectsWithTag("boton");
            if (listaObjetos.Length > 0)
            {   
                timer.CurrentTime = 0;
                timer.Started = true;

                for (int i = 0; i < listaObjetos.Length; i++)
                {
                    boton = listaObjetos[i];
                    manejadorTablero.instance.LlenarLista(ReadLevelTxt.ReadTxt(levelsList2[levelIndexList]), listaVer, i, boton);

                    if(i == 0){
                        contadorMovimientos=0;
                        contadorMovimientos+= boton.GetComponent<Giro>().clicks;
                    }else
                    {
                        contadorMovimientos+= boton.GetComponent<Giro>().clicks;
                    }
                }

                if (listaVer.Count == manejadorTablero.instance.Cols*manejadorTablero.instance.Cols-2)
                {
                    gameOver = true;
                }
            }

        }
    }
}