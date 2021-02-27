using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class AsteroidSpawner : MonoBehaviour
    {
        [SerializeField] GameObject asteroidPrefab;
        public void Spawn()
        {
            var asteroidInstance = Instantiate(asteroidPrefab);
            asteroidInstance.gameObject.GetComponent<Asteroid>().Speed = 2.5F;
            asteroidInstance.transform.position = transform.position;
        }

    }
}