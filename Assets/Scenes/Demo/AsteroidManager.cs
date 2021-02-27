using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class AsteroidManager : MonoBehaviour
    {
        [SerializeField] private List<AsteroidSpawner> spawnerList;
        private int maxSimultanealyActiveSpawners = 2;
        private float minSpawnSpeed = 1F;
        private float maxspawnSpeed = 3F;
        void Start()
        {
            StartCoroutine(StartSpawningCycle());
        }
         
        private IEnumerator StartSpawningCycle()
        {
            while (true)
            {
                var tempList = spawnerList.GetRange(0, spawnerList.Count);
                int activeSpawners = Random.Range(1, maxSimultanealyActiveSpawners + 1);
                for (int i = 0; i < activeSpawners; i++)
                {
                    int num = Random.Range(0, tempList.Count);
                    tempList[num].Spawn();
                    tempList.RemoveAt(num);
                }
                float spawnSpeed = Random.Range(minSpawnSpeed, maxspawnSpeed);
                yield return new WaitForSeconds(spawnSpeed);
            }
        }
    }
}