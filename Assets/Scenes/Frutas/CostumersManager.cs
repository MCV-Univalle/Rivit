using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Fruits
{
    public class CostumersManager : MonoBehaviour
    {
        [SerializeField] List<FrogSpawner> spawnersList = new List<FrogSpawner>();
        [SerializeField] private FrutalTree tree;
        private FruitsPickerGameManager _gameManager;

        [Inject]
        public void Init(GameManager manager)
        {
            _gameManager = (manager as FruitsPickerGameManager);
        }

        private float EstimateSelfdestructionTime(int activeSpawners)
        {
            float time = _gameManager.SelfDestructionTime;
            if (activeSpawners == 2)
                time = time + time * 0.5F;
            if (activeSpawners == 3)
                time = time * 2;

            return time;
        }

        private void Update()
        {
            if(tree.IsActive)
            {
                bool everySpawnerIsAvailable = true;
                foreach (FrogSpawner spawner in spawnersList)
                {
                    if (!spawner.IsAvailable())
                        everySpawnerIsAvailable = false;
                }
                if(everySpawnerIsAvailable)
                {
                    int activeSpawners = Random.Range(1, _gameManager.MaxActiveSpawners);
                    for (int i = 0; i < activeSpawners; i++)
                    {
                        int num = Random.Range(0, spawnersList.Count);
                        if(spawnersList[num].IsAvailable())
                        {
                            spawnersList[num].PrepareToSpawn(_gameManager.MinWaitTime, _gameManager.MaxWaitTime);
                            spawnersList[num].SelfDestructionTime = EstimateSelfdestructionTime(activeSpawners);
                        }
                            
                    }
                }
            }
        }
    }

}