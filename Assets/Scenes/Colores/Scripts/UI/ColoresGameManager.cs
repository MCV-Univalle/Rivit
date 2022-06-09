using System;
using System.Globalization;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;
using Random=UnityEngine.Random;
namespace Colores
{

    public class ColoresGameManager : ModeSystemGameManager
    {
        public GameObject guia;
        public GameObject bolitaExtra;
        public List<Color> colores;
        public List<string> palabras;
        public float tiempoEntreGuias {get; set;}
        public float tiempoEntreGuiasExtra {get; set;}
        public float comienzoDeTiempo {get; set;}
        public float comienzoDeTiempoExtra {get; set;}
        public float velocidad {get; set;}
        public float velocidadExtra {get; set;}
        public bool actualizar {get; set;}
        public GameObject panel;
        public int fallas {get; set;}
        public int errores {get; set;}
        public int correctos {get; set;}
        public int correctosSeguidos {get; set;}
        public int etapa {get; set;}
        public bool verificacion {get; set;}
        private int numColor,numPalabra;
        private int auxiliar = 2;
        private bool randomBoolean;
        public List<int> numeros;
        public GameObject botones;
        public GameObject señal;
        public GameObject ptjObtenido;

        private AdditionalDataColores additionalData;

        public override string Name => "Colores";
        [InjectOptional(Id = "SFXManager")] private AudioManager _SFXManager;

        public override void EndGame()
        {
            Debug.Log("EndGameEnter");
            auxiliar=0;
            botones.SetActive(false);
            señal.SetActive(false);
            ptjObtenido.SetActive(false);
            RaiseEndGameEvent();
        }

        public override string RegisterAdditionalData()
        {
            additionalData.Score = this.Score;
            return JsonConvert.SerializeObject(additionalData);
        }

        public override void IncreaseDifficulty()
        {
            _gameMode.IncreaseDifficulty(etapa);
        }
        public void LoadLevel()
        {   
            additionalData = new AdditionalDataColores();
            botones.SetActive(true);
            señal.SetActive(true);
            ptjObtenido.SetActive(true);

            switch(verificacion){
                case true:
                    if (tiempoEntreGuias <= 0){
                        GeneracionGuias.instance.Enemigos(guia, panel, colores, numColor, numPalabra,palabras);
                        tiempoEntreGuias = comienzoDeTiempo;
                    }
                break;
                
                case false:
                    if (tiempoEntreGuias <= 0){
                        GeneracionGuias.instance.Enemigos(guia, panel, colores, numColor, numPalabra,palabras);
                        tiempoEntreGuias = comienzoDeTiempo;
                    }

                    if(tiempoEntreGuiasExtra <= 0)
                    {
                    GeneracionGuias.instance.EnemigosExtra(bolitaExtra, panel, colores, numColor);
                    tiempoEntreGuiasExtra = comienzoDeTiempoExtra;
                    }

                    break;
            }

            /*if((!verificacion)&&(tiempoEntreGuiasExtra <= 0))
            {
                    GeneracionGuias.instance.EnemigosExtra(bolitaExtra, panel, colores, numColor);
                    tiempoEntreGuiasExtra = comienzoDeTiempoExtra;
            }*/
        }

        public override void StartGame()
        {
            Debug.Log("Metodo StartGame: Colores");
            //LoadLevel();
        }

        private void GameOver()
        { 
            EndGame();
            LeanTween.delayedCall( 0.5F, () => NotifyGameOver());
        }
        private void Start()
        {
            Debug.Log("Metodo Start: Colores");
        }

        public void ModoNormal(){

            if(correctos >= 10)
            {
                etapa += 1;
                correctos = 0;
            }

            if((fallas == 3)&&(etapa>1))
            {
                etapa -=1;
                correctos=0;
                errores+=1;

            }else if((fallas == 3)&&(etapa==1))
            {
                fallas = 0;
                errores+=1;
            }

            //error porque se llama en cada frame, arreglar eso
            if((errores == 3)&&(verificacion)&&(auxiliar==2)){
                actualizar = false;
                etapa = 0;
                auxiliar = 1;
            }

            if (actualizar)
                LoadLevel();

            if(auxiliar==1)
                GameOver();
        }


        public void ModoInfinity(){
            if(tiempoEntreGuiasExtra > 0)
                tiempoEntreGuiasExtra -= Time.deltaTime;

            if(correctos >= 10)
            {
                etapa += 1;
                correctos = 0;
            }

            if((fallas == 3)&&(etapa>1))
            {
                etapa -=1;
                correctos=0;

            }else if((fallas == 3)&&(etapa==1))
            {
                fallas = 0;
                errores+=1;
            }

            if (actualizar)
                LoadLevel();
        }

        private void Update()
        {
            numColor = Random.Range(0, 4);
            numeros.Remove(numColor);
            numPalabra = Random.Range(numeros[0], numeros.Count);
            numeros.Add(numColor);
            randomBoolean = Random.value < 0.15;
            
            if(tiempoEntreGuias > 0)
                tiempoEntreGuias -= Time.deltaTime;
            
            if(verificacion){
                ModoNormal();
            }else{
                ModoInfinity();
            }
        }

    }
}

