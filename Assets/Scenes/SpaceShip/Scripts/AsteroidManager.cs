using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class AsteroidManager : MonoBehaviour
    {
        [SerializeField] private List<AsteroidSpawner> spawnerList;
        [SerializeField] private GameObject warning;
        private int maxSimultanealyActiveSpawners = 2;
        private float minSpawnSpeed = 1F;
        private float maxspawnSpeed = 2F;

        public float AsteroidSpeed { get; set; }
        public float MinSpawnSpeed { get => minSpawnSpeed; set => minSpawnSpeed = value; }
        public float MaxSpawnSpeed { get => maxspawnSpeed; set => maxspawnSpeed = value; }

        void Start()
        {
        }
         
        public IEnumerator StartSpawningCycle()
        {
            yield return new WaitForSeconds(2F);
            while (true)
            {
                float spawnSpeed;
                int num = Random.Range(0, 4);
                if (num == 0)
                {
                    SpawnMeteorit();
                    spawnSpeed = MinSpawnSpeed;
                }
                else
                {
                    if (Random.Range(0, 7) == 0)
                        SpawnStar();
                    else
                        SpawnAsteroid();
                    spawnSpeed = Random.Range(MinSpawnSpeed, MaxSpawnSpeed);
                }
                yield return new WaitForSeconds(spawnSpeed);
            }
        }

        private void SpawnMeteorit()
        {
            int num = Random.Range(0, spawnerList.Count);
            if(!spawnerList[num].IsMeteoritOnHold && VerifyAvailableSpace())
            {
                var warningPos = spawnerList[num].transform.position - new Vector3(0, 3.5F, 0);
                Instantiate(warning, warningPos, Quaternion.identity);
                var spawner = spawnerList[num];
                spawner.IsMeteoritOnHold = true;
                LeanTween.delayedCall(gameObject, 1.5F, () => spawner.SpawnMeteorit());
            } 
        }

        private bool VerifyAvailableSpace()
        {
            bool available = true;
            int cont = 0;
            foreach (AsteroidSpawner item in spawnerList)
            {
                if (item.IsMeteoritOnHold) cont++;
                if (cont > 2) available = false;
            }
            return available;
        }

        private void SpawnAsteroid()
        {
            int num = Random.Range(0, spawnerList.Count);
            spawnerList[num].SpawnAsteroid();
        }

        private void SpawnStar()
        {
            int num = Random.Range(0, spawnerList.Count);
            spawnerList[num].SpawnStar();
        }
    }
}