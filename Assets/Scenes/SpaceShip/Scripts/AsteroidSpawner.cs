using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class AsteroidSpawner : MonoBehaviour
    {
        private float _speed = 4F;
        [SerializeField] private GameObject asteroidPrefab;
        [SerializeField] private GameObject meteoritPrefab;
        [SerializeField] private GameObject starPrefab;

        [SerializeField] private AsteroidManager asteroidManager;

        public bool IsMeteoritOnHold {get; set;}

        public void SpawnAsteroid()
        {
            var asteroidInstance = Instantiate(asteroidPrefab);
            asteroidInstance.gameObject.GetComponent<Asteroid>().Speed = asteroidManager.AsteroidSpeed;
            var rotation = new Vector3(0, 0, Random.Range(0, 300F));
            asteroidInstance.transform.Rotate(rotation);
            asteroidInstance.transform.position = transform.position;
        }
        public void SpawnMeteorit()
        {
            IsMeteoritOnHold = false;
            var asteroidInstance = Instantiate(meteoritPrefab);
            asteroidInstance.gameObject.GetComponent<Asteroid>().Speed = 0.4F;
            asteroidInstance.transform.position = transform.position;
        }

        public void SpawnStar()
        {
            var instance = Instantiate(starPrefab);
            instance.gameObject.GetComponent<Asteroid>().Speed = asteroidManager.AsteroidSpeed;
            instance.gameObject.GetComponent<Asteroid>().IsBonus = true;
            instance.transform.position = transform.position;
        }

    }
}