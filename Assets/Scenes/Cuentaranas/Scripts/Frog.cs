using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class Frog : MonoBehaviour
    {
        private Vector3 _initialPoint;
        private Vector3 _finalPoint;
        private Vector3 _middlePoint;
        private float _speed = 1f;
        private float _count = 0.0f;
        private SpriteManager spriteManager;
        [SerializeField] FrogSound frogSound;

        public bool IsJumping { get; set; }

        private void Awake()
        {
            spriteManager = new SpriteManager(GetComponent<SpriteRenderer>());
        }

        private void Start()
        {
            GameManager.endGame += ToRandomPosition;

        }
        private void OnDestroy()
        {
            GameManager.endGame -= ToRandomPosition;
        }

        private void Update()
        {
            Move();
        }

        public void PlaySound(AudioClip clip)
        {
            frogSound.PlayClip(clip);
        }

        public void ToRandomPosition()
        {
            IsJumping = false;
            _count = 0;
            transform.position = SelectRandomPoint();
        }
        public void Jump(float speed)
        {
            if (!IsJumping)
            {
                _speed = speed;
                CalcullateTrajectory();
                IsJumping = true;
            }
        }
        public void CalcullateTrajectory()
        {
            _initialPoint = DetermineInitialPoint();
            _finalPoint = DetermineFinalPoint();
            _middlePoint = DetermineMiddlePoint(_initialPoint, _finalPoint);
            spriteManager.Flip(_initialPoint, _finalPoint);
        }

        public Vector3 SelectRandomPoint()
        {
            float[] posibleX = { -5, 5 };
            float posX = posibleX[Random.Range(0, 2)];
            float posY = Random.Range(-2.5f, 3.5f);

            return new Vector3(posX, posY, 0);
        }

        public Vector3 DetermineInitialPoint()
        {
            if (transform.position == new Vector3(0, 0, 0))
                return new Vector3(0, 0, 0);
            else
                return SelectRandomPoint();
        }
        public Vector3 DetermineFinalPoint()
        {
            if (transform.position != new Vector3(0, 0, 0))
                return new Vector3(0, 0, 0);
            else
                return SelectRandomPoint();
        }

        public Vector3 DetermineMiddlePoint(Vector3 initialPoint, Vector3 finalPoint)
        {
            float posX = Random.Range(2.0f, 4.0f);
            if ((initialPoint.x < 0) || (finalPoint.x < 0))
                posX = posX * -1;
            float posY = Random.Range(0.5f, 6f);

            return new Vector3(posX, posY, 0);
        }

        public void Move()
        {
            if ((_count < 1f) && IsJumping)
            {
                _count += _speed * Time.deltaTime;

                Vector3 m1 = Vector3.Lerp(_initialPoint, _middlePoint, _count);
                Vector3 m2 = Vector3.Lerp(_middlePoint, _finalPoint, _count);

                transform.position = Vector3.Lerp(m1, m2, _count);

                if (_count > 0.5F || _count < 0.6F)
                    spriteManager.Rotate(gameObject, 30F, 0.5F);
            }
            else
            {
                IsJumping = false;
                _count = 0;
                gameObject.transform.Rotate(0, 0, 0);
            }
        }
    }
}
