using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class FrogsManager : MonoBehaviour
    {
        //Manage the velocity and make the frogs jump in or jump out

        private static FrogsManager _instance;
        public static FrogsManager Instance
        {
            get
            {
                //Logic to create the instance
                if(_instance == null)
                {
                    GameObject go = new GameObject("FrogsManager");
                    go.AddComponent<FrogsManager>();
                    _instance = go.GetComponent<FrogsManager>(); 
                }
                return _instance;
            }
        }

        [SerializeField]
        private GameObject[] _frogsList; //Edited in inspector
                                         //Assumptions: there would be only 9 frogs

        private int _remainingJumps = 20;
        public bool MakeQuestion {get; set;}

        void Awake()
        {
            _instance = this;
        }

         // Start is called before the first frame update
        void Start()
        {
            MakeQuestion = false;
        }

        public IEnumerator DetermineFrogs()
        {
            for(int i = _remainingJumps; i > 0; i--)
            {
                yield return new WaitForSeconds(1f);
                int num = Random.Range(1, 2);
                MakeJump(num);
                Debug.Log(CountFrogs());
            }
            MakeQuestion = true;
        }

        public void MakeJump(int num)
        {
            List<int> alreadySelected = new List<int>();
            for(int i = 0; i < num; i++)
            {
                int randNum = Random.Range(0, 12);
                if(randNum < 9)
                {
                    if(!_frogsList[randNum].GetComponent<CurvePath>().IsJumping)
                    {
                        if(!alreadySelected.Contains(randNum))
                        {
                            float spd = Random.Range(0.45f - 0.1f, 0.45f + 0.1f);
                            alreadySelected.Add(randNum);
                            _frogsList[randNum].GetComponent<CurvePath>().MakeJump(spd);
                        }
                    }
                }
            }

        }

        public int CountFrogs()
        {
            int cont = 0;
            foreach (var frog in _frogsList)
            {
                Vector3 vec = new Vector3(0, 0, 0);
                if(frog.GetComponent<CurvePath>().transform.position == vec) cont++;
            }
            return cont;
        }
       

        // Update is called once per frame
        void Update()
        {
            
        }
    }   
}
