using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShip
{
    public enum ShipState
    {
        Inactive,
        Idle,
        Moving,
        Dead
    }
    public class Ship : MonoBehaviour
    {

        private float shiftValue = 1.5F;
        private float moveSpeed = 0.050F;

        private ShipState state = ShipState.Idle;

        public bool HasBonus { get; set; }

        [SerializeField] private GameObject explosion;
        [SerializeField] private ParticleSystem particleSystem;

        public ShipState State { get => state; set => state = value; }

        private void Start()
        {
            GestureDetector.OnSwipe += DetectSwipe;
            State = ShipState.Inactive;
        }

        private void OnDestroy()
        {
            GestureDetector.OnSwipe -= DetectSwipe;
        }
        void Update()
        {
            if((State == ShipState.Dead || State == ShipState.Inactive) && gameObject.activeSelf)
            {
                this.gameObject.SetActive(false);
                particleSystem.gameObject.SetActive(false);
            }          
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Shift(-1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Shift(1);
            }
        }

        private void DetectSwipe(GestureDetector.SwipeData data)
        {
            int dir = 0;
            if (data.Direction == GestureDetector.Direction.Left)
                dir = -1;
            else if(data.Direction == GestureDetector.Direction.Right)
                dir = 1;
            Shift(dir);
        }

        private void Shift(int direction)
        {
            float finalShiftValue = shiftValue * direction;
            var finalPos = transform.position + new Vector3(finalShiftValue, 0, 0);
            if (VerifyIfPositionIsInsideLimits(finalPos) && State == ShipState.Idle)
            {
                Tween(finalPos);
            }   
        }

        private void Tween(Vector3 finalPos)
        {
            State = ShipState.Moving;
            LeanTween.move(gameObject, finalPos, moveSpeed);
            LeanTween.delayedCall(this.gameObject, moveSpeed, () => State = ShipState.Idle);
            
        }

        private bool VerifyIfPositionIsInsideLimits(Vector3 pos)
        {
            if (pos.x <= shiftValue && pos.x >= -shiftValue)
                return true;
            else return false;
        }

        public void Die()
        {
            var explosionInstance = Instantiate(explosion, this.transform.position, Quaternion.identity);
            LeanTween.delayedCall(this.gameObject, 0.9F, () => Destroy(explosionInstance));
            State = ShipState.Dead;
        }

        public void SpreadParticles()
        {
            particleSystem.gameObject.SetActive(true);
            particleSystem.Play();
        }
    }
}