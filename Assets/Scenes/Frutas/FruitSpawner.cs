using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fruits
{
    public class FruitSpawner : MonoBehaviour
    {
        [SerializeField] FruitsFactory fruitsFactory;
        private GameObject fruit;
        [SerializeField] bool active = true;
        private bool selected = false;
        private FrutalTree tree;

        public GameObject Fruit { get => fruit; set => fruit = value; }
        public bool IsSelected { get => selected; set => selected = value; }

        public GameObject Costumer { get; set; }
        bool IsActive { get => active; set => active = value; }

        private void Start()
        {
            IsSelected = false;
            tree = this.transform.parent.GetComponent<FrutalTree>();
        }

        public void Update()
        {
            if (fruit == null && active && tree.IsActive)
                SpawnFruit();
        }

        private FruitType GetAvailableFruitType()
        {
            int maxValue = (int)System.Enum.GetValues(typeof(FruitType)).Cast<FruitType>().Last();
            FruitType type = (FruitType)Random.Range(0, maxValue);

            while(!tree.IsAvailableFruit(type))
            {
                maxValue = (int)System.Enum.GetValues(typeof(FruitType)).Cast<FruitType>().Last();
                type = (FruitType)Random.Range(0, maxValue);
            }

            return type;
        }

        private void SpawnFruit()
        {
            FruitType type = GetAvailableFruitType();
            fruit = fruitsFactory.CreateFruit(type);
            fruit.transform.position = gameObject.transform.position;
        }
    }
}