using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fruits
{
    public class FruitPickerGameManager : ModeSystemGameManager
    {
        [SerializeField] private FrutalTree tree;
        public override string Name => "FruitsPicker";

        public object TimeSpawnGap { get; internal set; }
        public int SpawnRate { get; internal set; }
        public float SelfDestructionTime { get; internal set; }
        
        public override void EndGame()
        {
            tree.DesactiveSpawners();
            
        }

        public override void StartGame()
        {
            
        }

        

        public FruitSpawner SelectRandomFruit()
        {
            return tree.SelectRandomFruit();
        }
    }
}