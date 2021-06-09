using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class Asteroid : MonoBehaviour
    {
        public float Speed { get; set; }
        public bool IsBonus { get; set; }
        private void Start()
        {
            EnergyControl.energyRunOut += CancelTween;
            GameManager.endGame += SelfDestroy;
            var currentPos = transform.position;
            var finalPos = currentPos + new Vector3(0, -15F, 0);
            LeanTween.move(gameObject, finalPos, Speed);
            LeanTween.delayedCall(gameObject, Speed, () => Destroy(gameObject));
        }

        private void OnDestroy()
        {
            EnergyControl.energyRunOut -= CancelTween;
            GameManager.endGame -= SelfDestroy;
        }

        private void CancelTween()
        {
            LeanTween.pause(this.gameObject);
        }

        private void SelfDestroy()
        {
            Destroy(this.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.transform.tag == "Player")
            {
                if (IsBonus)
                {
                    collision.GetComponent<Ship>().HasBonus = true;
                    Destroy(this.gameObject);
                }           
                else
                {
                    Destroy(this.gameObject);
                    collision.GetComponent<Ship>().Die();
                }
            }
        }
    }
}