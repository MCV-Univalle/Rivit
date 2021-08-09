using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Fruits
{
    public class FrogSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject costumerPrefab;
        [SerializeField] private FruitsFactory factory;
        [SerializeField] private FrutalTree tree;

        private GameObject costumer;

        [SerializeField] private FruitsPickerGameManager gameManager;
        [Inject(Id = "SFXManager")] private AudioManager SFXManager;

        public float SelfDestructionTime { get; set; }

        private IEnumerator spawnCoroutine;

        private void Start()
        {
            GameManager.endGame += AbortCoroutine;
        }
        private void OnDestroy()
        {
            GameManager.endGame -= AbortCoroutine;
        }

        private void DestroyFrog()
        {
            if (costumer != null)
                Destroy(costumer);
        }

        private void AbortCoroutine()
        {
            if(spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);
            DestroyFrog();
        }

        public bool IsAvailable()
        {
            if (spawnCoroutine == null && (costumer == null || costumer.GetComponent<Frog>().Finished))
                return true;
            else return false;
        }

        public void PrepareToSpawn(float minWaitTime, float maxWaitTime)
        {
            spawnCoroutine = WaitAndSpawn(minWaitTime, maxWaitTime);
            StartCoroutine(spawnCoroutine);
        }

        private IEnumerator WaitAndSpawn(float minWaitTime, float maxWaitTime)
        {
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);
            Spawn();
        }

        private FruitSpawner GetAvailableSpawner()
        {
            int num = Random.Range(0, gameManager.MaxActiveFruits);
            FruitSpawner target = tree.SpawnerList[num];

            while (target.Costumer != null)
            {
                num = Random.Range(0, gameManager.MaxActiveFruits);
                target = tree.SpawnerList[num];
            }
            return target;
        }
        private void Spawn()
        {
            spawnCoroutine = null;
            var target = GetAvailableSpawner();
            SFXManager.PlayAudio("Boing");
            FruitType type = target.Fruit.GetComponent<Fruit>().Type;
            var temp = Instantiate(costumerPrefab).transform.GetChild(0).GetComponent<Frog>();
            temp.DesiredFruit = type;
            target.Costumer = temp.gameObject;
            temp.TargetedSpanwer = target;
            temp.SFXManager = this.SFXManager;
            temp.FruitsGameManager = gameManager;
            temp.transform.parent.position = gameObject.transform.position;
            var sprite = factory.LoadSprite(type);
            temp.Fruit.GetComponent<SpriteRenderer>().sprite = sprite;
            temp.SelfDestructionTime = this.SelfDestructionTime;
            costumer = temp.gameObject;
        }
    }
}