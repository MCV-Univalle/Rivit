using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class ScoreTrigger : MonoBehaviour
    {
        [SerializeField] SpaceShipGameManager gameManager;
        private bool active = true;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.transform.tag == "Asteroid" && active)
            {
                gameManager.Score++;
                active = false;
                LeanTween.delayedCall(this.gameObject, 0.5F, () => active = true);
            }
                
        }
    }
}