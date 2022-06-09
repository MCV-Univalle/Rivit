using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorsFrogCounter
{
    public class Frog : MonoBehaviour
    {
        public bool FixedPosition { get; set; }

        private void Start()
        {
            FixedPosition = false;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.GetComponent<Frog>().FixedPosition)
            {
                collision.gameObject.GetComponent<Frog>().AppearInRandomPosition();
            }
            
        }

        public void AppearInRandomPosition()
        {
            float posX = Random.Range(-2.15F, 2.15F);
            float posY = Random.Range(-4.6F, 3.8F);
            transform.position = new Vector3(posX, posY, 0);
        }
    }
}