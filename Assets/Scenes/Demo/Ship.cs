using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public enum ShipState
    {
        Idle,
        Moving,
        Dead
    }
    public class Ship : MonoBehaviour
    {

        private float shiftValue = 1.5F;
        private float moveSpeed = 0.1F;

        private ShipState state = ShipState.Idle;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Shift(-1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Shift(1);
            }
        }

        private void Shift(int direction)
        {
            float finalShiftValue = shiftValue * direction;
            var finalPos = transform.position + new Vector3(finalShiftValue, 0, 0);
            if (VerifyIfPositionIsInsideLimits(finalPos) && state == ShipState.Idle)
            {
                Tween(finalPos);
            }   
        }

        private void Tween(Vector3 finalPos)
        {
            state = ShipState.Moving;
            LeanTween.move(gameObject, finalPos, moveSpeed);
            LeanTween.delayedCall(this.gameObject, moveSpeed, () => state = ShipState.Idle);
            
        }

        private bool VerifyIfPositionIsInsideLimits(Vector3 pos)
        {
            if (pos.x <= shiftValue && pos.x >= -shiftValue)
                return true;
            else return false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.transform.tag == "Asteroid")
            {
                Destroy(collision.gameObject);
                Destroy(gameObject);
            }
        }
    }
}