using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fruits
{
    public class FrutalTree : MonoBehaviour
    {
        [SerializeField] List<FruitSpawner> spawnerList = new List<FruitSpawner>();

        public List<FruitSpawner> SpawnerList { get => spawnerList;}
        public bool IsActive { get; set; }

        private void Awake()
        {
            foreach(Transform child in transform)
            {
                spawnerList.Add(child.gameObject.GetComponent<FruitSpawner>());
            }
        }

        public bool IsAvailableFruit(FruitType fruit)
        {
            bool isAvailable = true;
            foreach (var spawner in spawnerList)
            {
                var temp = spawner.Fruit;
                if(temp != null)
                    if (temp.GetComponent<Fruit>().Type == fruit) isAvailable = false; 
            }
            return isAvailable;
        }

        public FruitSpawner SelectRandomFruit()
        {
            int num = Random.Range(0, spawnerList.Count);
            while (spawnerList[num].IsSelected == true)
            {
                num = Random.Range(0, spawnerList.Count);
            }
            spawnerList[num].IsSelected = true;
            return spawnerList[num];

        }
    }
}