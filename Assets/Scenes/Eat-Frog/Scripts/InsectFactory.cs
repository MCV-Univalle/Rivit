using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Eat_frog_Game
{
    public class InsectFactory : MonoBehaviour
    {
        public GameObject[] bees;
        public GameObject frog;
        private InsectEaterGameManager _gameManager;

        [Inject]
        public void Construct(GameManager gameManager)
        {
            this._gameManager = gameManager as InsectEaterGameManager;
        }


        private int cont;

        public float tiempocreate = 2f, rango = 2f;

        void Awake()
        {


        }
        // Start is called before the first frame update

        void Start()
        {
            InvokeRepeating("Create", 0.0f, tiempocreate);
        }

        public void Create()
        {
            Vector3 spawsposition = new Vector3(0, 0, 0);
            spawsposition = this.transform.position + Random.onUnitSphere * rango;
            spawsposition = new Vector3(this.transform.position.x, spawsposition.y, 0);
            Insect insect;
            List<int> numberinsects = new List<int> { };
            if (frog.transform.position.y + 1 <= spawsposition.y && _gameManager.Active)
            {
                if (_gameManager.Score >= 1500)
                {
                    insect = Instantiate(bees[Random.Range(0, 5)], transform.position, Quaternion.identity).GetComponent<Insect>();
                    insect.GameManager = _gameManager;
                }
                else
                {
                    if (_gameManager.Score >= 500)
                    {
                        insect = Instantiate(bees[Random.Range(0, 4)], transform.position, Quaternion.identity).GetComponent<Insect>();
                        insect.GameManager = _gameManager;
                    }
                    else
                    {
                        if (_gameManager.Score >= 50)
                        {
                            numberinsects.Clear();
                            if (numberinsects.Count == 0)
                            {
                                numberinsects.Add(0);
                                numberinsects.Add(1);
                                numberinsects.Add(2);
                            }
                            int numb = Random.Range(0, numberinsects.Count);
                            insect = Instantiate(bees[numb], spawsposition, Quaternion.identity).GetComponent<Insect>();
                            insect.GameManager = _gameManager;
                            numberinsects.Remove(numberinsects[numb]);


                        }
                        else
                        {
                            if (numberinsects.Count == 0)
                            {
                                numberinsects.Add(0);
                                numberinsects.Add(1);
                            }
                            int numb = Random.Range(0, numberinsects.Count);
                            insect = Instantiate(bees[numb], spawsposition, Quaternion.identity).GetComponent<Insect>();
                            insect.GameManager = _gameManager;
                            numberinsects.Remove(numberinsects[numb]);
                        }
                    }
                }
            }
            
        }

    }

}
