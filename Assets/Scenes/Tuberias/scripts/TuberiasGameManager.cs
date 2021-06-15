using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;
using Tuberias;

namespace Tuberias
{
    public class TuberiasGameManager : LevelSystemGameManager
    {
        public override string Name => "Tuberias";
        [InjectOptional(Id = "SFXManager")] private AudioManager _SFXManager;
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
        private GameObject boton;
        public int cantidad;
        public GameObject panelLevelComplete;

        public override void EndGame()
        {
            Debug.Log("EndGameEnter");
        }

        public override void StartGame()
        {
            Debug.Log("Metodo Start: Tuberias");
            
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
        }

        public void LoadLevel()
        {
            validar = 1;
            nivelCompletado.SetActive(false);
            tablero.SetActive(true);
            TuberiasUIManager.instance.SetLevelTmpLabel();
            tamaño = manejadorTablero.instance.Cols*manejadorTablero.instance.Rows;
            panelLevelComplete.SetActive(false);
        
        }
         
        public void ReLoadLevel()
        {

        }

        private void GameOver()
        {
            listaObjetos = GameObject.FindGameObjectsWithTag("boton");
            if (listaObjetos.Length > 0)
            {   
                for (int i = 0; i < tamaño; i++)
                {
                    boton = listaObjetos[i];
                    manejadorTablero.instance.LlenarLista(ReadLevelTxt.ReadTxt(levelsList2[levelIndexList]), listaVer, i, boton);
                }

                if (listaVer.Count == tamaño-2)
                    validar = 2;

                if (validar==2)
                {
                    print("Game Over: Gano");
                    tablero.SetActive(false);
                    nivelCompletado.SetActive(true);
                    levelIndexList++;
                    manejadorTablero.instance.angulos.Clear();
                    panelLevelComplete.SetActive(true);
                    validar = 0;
                    //manejadorTablero.instance.AcomodarTablero(ReadLevelTxt.ReadTxt(levelsList2[levelIndexList]));
                
                    //LoadLevel();
                }
            }

        }
    }
}

