using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fruits
{
    public class FrogSpawner : MonoBehaviour
    {
        [SerializeField] private FrutalTree tree;
        [SerializeField] private GameObject costumerPrefab;
        [SerializeField] private FruitsFactory factory;

        private GameObject costumer;

        [SerializeField] private FruitsPickerGameManager gameManager;

        public float TimeSpawnGap { get => gameManager.TimeSpawnGap; }
        public int SpawnRate { get => gameManager.SpawnRate; }
        public float SelfDestructionTime { get => gameManager.SelfDestructionTime; }

        private IEnumerator spawnCoroutine;

        // Update is called once per frame
        void Update()
        {
            if (costumer == null && spawnCoroutine == null && tree.IsActive)
            {
                spawnCoroutine = DecideWhenToSpawn();
                StartCoroutine(spawnCoroutine);
            }

        }

        private IEnumerator DecideWhenToSpawn()
        {
            bool value = true;
            yield return new WaitForSeconds(1F);

            while (value)
            {
                if (Random.Range(0, SpawnRate) == 0)
                {
                    spawnCoroutine = null;
                    Spawn();
                    value = false;
                }
                else
                    yield return new WaitForSeconds(TimeSpawnGap);
            }
        }
        private FruitSpawner GetAvailableSpawner()
        {
            int num = Random.Range(0, tree.SpawnerList.Count);
            FruitSpawner target = tree.SpawnerList[num];

            while (target.Costumer != null)
            {
                num = Random.Range(0, tree.SpawnerList.Count);
                target = tree.SpawnerList[num];
            }
            return target;
        }
        private void Spawn()
        {
            var target = GetAvailableSpawner(); 
            FruitType type = target.Fruit.GetComponent<Fruit>().Type;
            var temp = Instantiate(costumerPrefab).GetComponent<Frog>();
            temp.DesiredFruit = type;
            target.Costumer = temp.gameObject;
            temp.TargetedSpanwer = target;
            temp.transform.position = gameObject.transform.position;
            var sprite = factory.LoadSprite(type);
            temp.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sprite;
            temp.WaitAndDestroy(SelfDestructionTime);
            costumer = temp.gameObject;
        }
    }
}