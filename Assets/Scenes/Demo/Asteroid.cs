using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class Asteroid : MonoBehaviour
    {
        public float Speed { get; set; }
        private void Start()
        {
            var currentPos = transform.position;
            var finalPos = currentPos + new Vector3(0, -15F, 0);
            LeanTween.move(gameObject, finalPos, Speed);
            LeanTween.delayedCall(gameObject, Speed, () => Destroy(gameObject));
        }
    }
}