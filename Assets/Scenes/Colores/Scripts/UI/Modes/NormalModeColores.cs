using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using System.Diagnostics;

 using Debug = UnityEngine.Debug;

namespace Colores
{
    public class NormalModeColores : ColoresGameMode
    {
        public override void InitializeSettings()
        {
            var gameManager = (ColoresGameManager)_gameManager;
            gameManager.tiempoEntreGuias = 0;
            gameManager.actualizar = true;
            gameManager.etapa = 1;
            gameManager.fallas = 0;
            gameManager.correctos = 0;  
            gameManager.errores = 0;
            gameManager.verificacion = true;
        }

        public override void IncreaseDifficulty(int etapa)
        {
            var gameManager = (ColoresGameManager)_gameManager;
            
            gameManager.comienzoDeTiempo = CambiarFrecuencia(etapa);
            gameManager.velocidad = CambiarVelocidad(etapa);
        }
        public float CambiarFrecuencia(int etapa)
        {
            switch (etapa)
            {
                case 1:
                    return 6;
                    break;

                case 2:
                    return 4;
                    break;

                case 3:
                    return 2;
                    break;

                case 4:
                    return 1;
                    break;
            }

            return 0;
        }

        public float CambiarVelocidad(int etapa)
        {
            switch (etapa)
            {
                case 1:
                    return 1;
                    break;

                case 2:
                    return 2;
                    break;

                case 3:
                    return 3;
                    break;

                case 4:
                    return 4;
                    break;
            }

            return 0;
        }
    }
}