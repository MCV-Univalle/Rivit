using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fruits
{
    public class FruitsFactory : MonoBehaviour
    {
        [SerializeField] private GameObject fruitPrefab;
        readonly string resourcesPath = "Fruits/";


        public GameObject CreateFruit(FruitType type)
        {
            var fruitIntance = Instantiate(fruitPrefab);
            fruitIntance.gameObject.GetComponent<Fruit>().Type = type;
            var sprite = LoadSprite(type);
            fruitIntance.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;

            return fruitIntance;
        }

        public Sprite LoadSprite(FruitType type)
        {
            return Resources.Load<Sprite>(resourcesPath + type.ToString());
        }
        
    }
}