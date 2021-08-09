using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;
using Tuberias;
using System.Diagnostics;
 using Debug = UnityEngine.Debug;

namespace Tuberias
{
    public class TuberiasGameManager : LevelSystemGameManager
    {
        public override string Name => "Tuberias";
        [InjectOptional(Id = "SFXManager")] private AudioManager SFXManager;
        public int LevelIndexList { get => levelIndexList; set => levelIndexList = value; }
        public List<Object> LevelsList { get => levelsList; set => levelsList = value; }
        public int Validar { get => validar; set => validar = value; }
        public int levelIndexList;
        public int tamaño;
        public GameObject tablero;
        public GameObject nivelCompletado;
        public List<Object> levelsList = new List<Object>();
        public List<TextAsset> levelsList2;
        public GameObject[] listaObjetos;
        public List<int> listaVer;
        private int validar = 0;
        private int ValidarAudio = 0;
        private float tiempoEspera = 1f;
        private GameObject boton;
        public int cantidad;
        public GameObject panelLevelComplete;
        private bool gameOver;

        public override void EndGame()
        {
            Debug.Log("EndGameEnter");
            validar=0;
            tamaño=2;
            TuberiasUIManager.instance.ActivePanelControlsTuberias(false);
        }
        public override void StartGame()
        {
            Debug.Log("Metodo Start: Tuberias");
            gameOver = false;
            levelIndexList = 0;

            LoadLevel();
        }

        private void Start()
        {
            Debug.Log("Metodo Start: Tuberias");
        }

        private void Update()
        {
            GameOver();

            if(gameOver)
            {
                TuberiasUIManager.instance.ActivePanelGameOver(gameOver);
                TuberiasUIManager.instance.EraseBoard(false);
            }

        }

        public void LoadLevel()
        {
            validar = 1;
            ValidarAudio=1;
            //nivelCompletado.SetActive(false);
            TuberiasUIManager.instance.EraseBoard(true);
            tablero.SetActive(true);
            //TuberiasUIManager.instance.SetLevelTmpLabel();
            tamaño = manejadorTablero.instance.Cols*manejadorTablero.instance.Rows;
            //panelLevelComplete.SetActive(false);
        
        }
         
        public void ReLoadLevel()
        {

        }

        private void GameOver()
        {
            listaObjetos = GameObject.FindGameObjectsWithTag("boton");
            if (listaObjetos.Length > 0)
            {   
                for (int i = 0; i < listaObjetos.Length; i++)
                {
                    boton = listaObjetos[i];
                    manejadorTablero.instance.LlenarLista(ReadLevelTxt.ReadTxt(levelsList2[levelIndexList]), listaVer, i, boton);
                }
               
                if (listaVer.Count == tamaño-2)
                    gameOver = true;


                if(validar==2){

                    //SFXManager.PlayAudio("Correct");

                   /* for(int i=0; i<tamaño;i++)
                        manejadorTablero.instance.CambioPiezas(listaObjetos[i]);
                    */
                    print("Game Over: Gano");
                    manejadorTablero.instance.angulos.Clear();

                    EndGame();

                    //StartCoroutine(FinalizarJuego());
                    //manejadorTablero.instance.AcomodarTablero(ReadLevelTxt.ReadTxt(levelsList2[levelIndexList]));
                    //LoadLevel();
                }
            }

        }


        void acabarJuego(){
            //tablero.SetActive(false);
            //panelLevelComplete.SetActive(true);
            EndGame();
        }
    }
}

