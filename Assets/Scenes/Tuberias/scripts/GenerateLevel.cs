using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UI;
using Tuberias;

namespace Tuberias
{
    public class GenerateLevel : MonoBehaviour, ILevelGenerator
    {
        void ILevelGenerator.GenerateLevel(List<string[]> nivel)
        {
            manejadorTablero.instance.AcomodarTablero(nivel);
            TuberiasUIManager.instance.ActivePanelGameOver(false);
        }

    }

}
