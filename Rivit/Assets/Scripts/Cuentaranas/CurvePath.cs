using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class CurvePath : MonoBehaviour
    {
        //This script makes the traslation of an gameobject following a Bezier Curve with three points.

        [SerializeField]
        private Vector3[] _points; //Edited in the inspector
                                    //It is supoused to be 7 differentes points
                                    /*
                                    ---___________---
                                    ---|         |---
                                    x--|         |--x
                                    x--|    x    |--x
                                    x--|         |--x
                                    ---|         |---
                                    ---___________---
                                    */

        private Vector3 _inBetweenPoint;
        private float _count = 0.0f;
        private float _speed = 1f;
        public bool IsJumping {get; set;}
        private bool _isInitialPoint = true;
        public int _currentPoint = 0;

        public void MakeJump(float speed)
        {
            IsJumping = true;
            _count = 0.0f;
            _isInitialPoint = DetermineDirection();
            _speed = speed;
        }

        public Vector3 DefineInBetweenPoint()
        {
            float randNumX;
            if(_currentPoint <= 3)
            randNumX = Random.Range(-2.0f, -4.0f);
            else
            randNumX = Random.Range(2.0f, 4.0f);

            float randNumY = Random.Range(0.5f, 6f);
            Vector3 vec = new Vector3(randNumX, randNumY, 0f);
            return vec;
        }

        public bool DetermineDirection() //Determine if the frog will travel from central point to another point,
                                        // or from another point to central point
        {
            _currentPoint = Random.Range(1, 7);
            _inBetweenPoint = DefineInBetweenPoint();

            if(transform.position == _points[0])
            return true;
            else 
            return false;
        }

        public void CurveInterpolation()
        {
            if(_count < 1f)
            {
                _count += _speed * Time.deltaTime;
                Vector3 m1;
                Vector3 m2;

                if(_isInitialPoint)
                {
                    
                    m1 = Vector3.Lerp(_points[0], _inBetweenPoint, _count);
                    m2 = Vector3.Lerp(_inBetweenPoint, _points[_currentPoint], _count);
                }
                else
                {
                    m1 = Vector3.Lerp(_points[_currentPoint], _inBetweenPoint, _count);
                    m2 = Vector3.Lerp(_inBetweenPoint, _points[0], _count);
                }
                transform.position = Vector3.Lerp(m1, m2, _count);
            }
            else 
            {
                IsJumping = false;
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            _inBetweenPoint = DefineInBetweenPoint();
        }

        // Update is called once per frame
        void Update()
        {
            if(IsJumping)
            CurveInterpolation();
        }
    }   
}
