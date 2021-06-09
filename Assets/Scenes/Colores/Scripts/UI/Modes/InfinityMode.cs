using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;
namespace Colores
{
    public class InfinityMode : ColoresGameMode
    {
        public override void InitializeSettings()
        {
            var gameManager = (ColoresGameManager)_gameManager;

            gameManager.tiempoEntreGuias = 0;
            gameManager.actualizar = true;
            gameManager.etapa = 1;
            gameManager.fallas = 0;
            gameManager.correctos = 0;  
            gameManager.errores = 4;
            gameManager.verificacion = false;
        }
        public override void IncreaseDifficulty(int etapa)
        {
            var gameManager = (ColoresGameManager)_gameManager;

            gameManager.comienzoDeTiempo = CambiarFrecuencia(etapa);
            gameManager.velocidad = CambiarVelocidad(etapa);
        
            gameManager.velocidadExtra = CambiarVelocidad(etapa)+30;
            gameManager.comienzoDeTiempoExtra = Random.Range(1,11);
        }

        public float CambiarFrecuencia(int etapa)
        {
            switch (etapa)
            {
                case 1:
                    return 8;
                    break;

                case 2:
                    return 6;
                    break;

                case 3:
                    return 3;
                    break;

                case 4:
                    return 1.5f;
                    break;
            }

            return 0;
        }

        public float CambiarVelocidad(int etapa)
        {
            switch (etapa)
            {
                case 1:
                    return 70;
                    break;

                case 2:
                    return 100;
                    break;

                case 3:
                    return 130;
                    break;

                case 4:
                    return 160;
                    break;
            }

            return 0;
        }
    }
}
