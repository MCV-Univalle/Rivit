using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eat_frog_Game
{
    public class Insect : MonoBehaviour
    {
        // Start is called before the first frame updat
        private int zigZagMove, typeMove;
        public int Resistencia;
        public float Timee;
        public float vel;
        private Objective Objective;
        private bool Change_Direction, Invisiblee;

        private InsectEaterGameManager _gameManager;

        public InsectEaterGameManager GameManager { get => _gameManager; set => _gameManager = value; }

        void Awake()
        {

            Objective = FindObjectOfType<Objective>();

        }

        void Start()
        {
            Change_Direction = true;
            Timee = 0f;
            Invisiblee = false;
            zigZagMove = 0;
            Resistencia = 0;
            vel = 1.25f;
            typeMove = Random.Range(1, 3);
        }

        // Update is called once per frame
        void Update()
        {
            Move();
            velTypeBee();
            special();
        }

        void special()
        {
            if (gameObject.tag == "Mariposa_dorada")
            {
                Timee += Time.deltaTime;
                if (!Invisiblee)
                {
                    if (Timee >= Random.Range(1, 2))
                    {
                        gameObject.GetComponent<SpriteRenderer>().sortingOrder = -1;
                        Timee = 0;
                        Invisiblee = true;
                    }
                }
                else
                {
                    if (Timee >= Random.Range(1, 2))
                    {
                        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        Timee = 0;
                        Invisiblee = false;
                    }
                }


            }
        }
        void velTypeBee()
        {
            if (gameObject.tag == "Roja")
            {
                vel = 0.9f;
            }
            if (gameObject.tag == "Blanca")
            {
                vel = 0.8f;
            }
        }

        void aumVel()
        {
            if (_gameManager.Score >= 150)
            {
                vel += 0.5f;
            }
        }

        void Move()
        {
            switch (typeMove)
            {
                case 1:
                    transform.position += transform.right * vel * Time.deltaTime;

                    if (Change_Direction)
                    {
                        transform.position += transform.up * vel * Time.deltaTime;
                        zigZagMove += 1;
                        if (zigZagMove == 50)
                        {
                            Change_Direction = false;
                        }
                    }
                    else
                    {
                        transform.position -= transform.up * vel * Time.deltaTime;
                        zigZagMove -= 1;
                        if (zigZagMove == 0)
                        {
                            Change_Direction = true; ;
                        }
                    }
                    break;
                case 2:
                    transform.position += transform.right * vel * Time.deltaTime;
                    break;
            }

        }

        void OnTriggerEnter2D(Collider2D other)
        {

            if (other.gameObject.tag == "lengua")
            {
                bool lim = _gameManager.limitreached;
                if (!lim)
                {
                    if ((gameObject.tag == "Blanca") || (gameObject.tag == "Roja"))
                    {
                        Objective.Resistencia = Resistencia;
                        Destroy(gameObject);
                    }
                    if (gameObject.tag == "Negra" || gameObject.tag == "Cafe" || gameObject.tag == "Mariposa_dorada")
                    {
                        Destroy(gameObject);
                    }
                }

            }

            if (other.gameObject.tag == "Destroyer")
            {
                Destroy(gameObject);
            }
        }
    }

}
