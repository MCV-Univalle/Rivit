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

        public int RemainingJumps = 8;
        public int ActiveFrogsNumber = 1; //The maxime number of frogs that will be jumping
        public float NormalSpeed = 0.35f;
        public float SpeedVariance = 0.04f; //Every frog will be a slightly different speed when jumping
        public float WaitTime = 2.25f;
        public int JumpingRatio = 10; //If jumping ratio is 10, every frog will jump
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
            for(int i = RemainingJumps; i > 0; i--)
            {
                int num = Random.Range(1, ActiveFrogsNumber);
                MakeJump(num);
                yield return new WaitForSeconds(WaitTime);
            }
            MakeQuestion = true;
        }

        public void MakeJump(int num)
        {
            List<int> alreadySelected = new List<int>();
            for(int i = 0; i < num; i++)
            {
                int selectedFrog = Random.Range(0, 9);
                int randNum = Random.Range(0, 10);
                
                if(!_frogsList[selectedFrog].GetComponent<CurvePath>().IsJumping)
                {
                    if(!alreadySelected.Contains(selectedFrog))
                    {
                        float spd = Random.Range(NormalSpeed - SpeedVariance, NormalSpeed + SpeedVariance);
                        alreadySelected.Add(selectedFrog);
                        _frogsList[selectedFrog].GetComponent<CurvePath>().MakeJump(spd);
                        
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

        public void ReturnEveryFrogToOriginalPosition()
        {
            foreach(var frog in _frogsList)
            {
                frog.GetComponent<CurvePath>().ReturnToOriginalPosition();
            }
        }

        public void SetActiveEveryFrog(bool value)
        {
            foreach(var frog in _frogsList)
            {
                frog.gameObject.SetActive(value);
            }
        }
       

        // Update is called once per frame
        void Update()
        {
            
        }
    }   
}
