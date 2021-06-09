using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFreeV2
{
    public class GenerateLevel : MonoBehaviour, ILevelGenerator
    {
        void ILevelGenerator.GenerateLevel(List<string[]> level)
        {
            LoadLevel(level);
        }

        public void LoadLevel(List<string[]> level)
        {
            GenerateBoard._instance.CreateBoard(level);
        }
    }
}

