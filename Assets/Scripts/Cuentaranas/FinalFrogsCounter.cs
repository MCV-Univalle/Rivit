using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class FinalFrogsCounter : MonoBehaviour
    {
        private static FinalFrogsCounter _instance;
            public static FinalFrogsCounter Instance
            {
                get
                {
                    //Logic to create the instance
                    if(_instance == null)
                    {
                        GameObject go = new GameObject("FinalFrogsCounter");
                        go.AddComponent<FinalFrogsCounter>();
                        _instance = go.GetComponent<FinalFrogsCounter>(); 
                    }
                    return _instance;
                }
            }

        [SerializeField]
        private GameObject _layoutA;
        [SerializeField]
        private GameObject _layoutB;
        [SerializeField]
        private GameObject _layoutC;

        [SerializeField]
        private GameObject _frogPrefab;

        void Awake()
        {
            _instance = this;
        }

        public void PutActualFrogs(int num)
        //Assuming the maximun number of frogs is 9

        {
            for(int i = 0; i < num; i++)
            {
                GameObject go = Instantiate(_frogPrefab) as GameObject;
                if(i == 8)
                go.transform.SetParent(_layoutA.transform, false);
                else if(i > 4)
                go.transform.SetParent(_layoutB.transform, false);
                else
                go.transform.SetParent(_layoutC.transform, false);

                go.gameObject.SetActive(false);
            }
            StartCoroutine(StartCountingFrogs(num));
        }

        public IEnumerator StartCountingFrogs(int num)
        {
            Transform childrenListA = _layoutA.transform;
            Transform childrenListB = _layoutB.transform;
            Transform childrenListC = _layoutC.transform;

            yield return new WaitForSeconds(0.3f);

            foreach (Transform frog in childrenListC)
            {
                frog.gameObject.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.25f);
            }
            foreach (Transform frog in childrenListB)
            {
                frog.gameObject.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.25f);
            }
            foreach (Transform frog in childrenListA)
            {
                frog.gameObject.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.25f);
            }
            
            yield return new WaitForSeconds(1.5f);

            FinishDisplay();
        }

        public void FinishDisplay()
        {
            DestroyFrogs();
            GameManager.Instance.StartNewIteration();
        }

        public void DestroyFrogs()
        {
            Transform childrenListA = _layoutA.transform;
            Transform childrenListB = _layoutB.transform;
            Transform childrenListC = _layoutC.transform;

            foreach (Transform frog in childrenListA)
            {
                GameObject frogObject = frog.gameObject;
                Destroy(frogObject);
            }
            foreach (Transform frog in childrenListB)
            {
                GameObject frogObject = frog.gameObject;
                Destroy(frogObject);
            }
            foreach (Transform frog in childrenListC)
            {
                GameObject frogObject = frog.gameObject;
                Destroy(frogObject);
            }
        }
    }   
}
