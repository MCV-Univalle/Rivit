using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public class BackgroundElement : MonoBehaviour
    {
        [SerializeField] private float speed;
        private float heigth;
        private float initialPos;
        private float finalPos;
        // Start is called before the first frame update
        void Start()
        {
            heigth = this.GetComponent<SpriteRenderer>().bounds.size.y;
            initialPos = 0 + heigth;
            finalPos = 0 - heigth;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 velocity = Vector3.zero;
            Vector3 desiredPosition = transform.position + new Vector3(0, -speed, 0);
            Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 0.3f);
            transform.position = smoothPosition;

            if (transform.position.y < finalPos)
                transform.position = new Vector3(0,initialPos,0);
        }
    }
}