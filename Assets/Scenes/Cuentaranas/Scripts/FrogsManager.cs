using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Cuentaranas
{
    public class FrogsManager : MonoBehaviour
    {
        [SerializeField] FrogsCounterGameManager _gameManager;
        [SerializeField] private GameObject frogPrefab;
        [SerializeField] private int numberFrogs = 9;
        private List<GameObject> frogsList;
        public int ActiveFrogsNumber { get; set; }

        private void Awake()
        {
            InstantiateFrogs();
        }

        private void Start()
        {
            ToRandomPosition();
        }

        public void InstantiateFrogs()
        {
            frogsList = new List<GameObject>();
            for (int i = 0; i < 9; i++)
            {
                GameObject go = Instantiate(frogPrefab);
                frogsList.Add(go);
                go.transform.parent = this.transform;
            }
        }

        public void ToRandomPosition()
        {
            foreach (var frog in frogsList)
            {
                frog.GetComponent<Frog>().ToRandomPosition();
            }
        }

        public IEnumerator StartJumping(int reps, float waitTime, float normalSpeed, float speedVariance)
        {
            yield return new WaitForSeconds(waitTime);
            for (int i = reps; i > 0; i--)
            {
                int num = Random.Range(1, ActiveFrogsNumber);
                MakeJump(num, normalSpeed, speedVariance);
                yield return new WaitForSeconds(waitTime);
            }
            yield return new WaitForSeconds(waitTime * 2);
            _gameManager.ShowQuestionPanel();
        }

        public void MakeJump(int num, float normalSpeed, float speedVariance)
        {
            for (int i = 0; i < num; i++)
            {
                int selectedFrog = SelectFrog();

                float speed = Random.Range(normalSpeed - speedVariance, normalSpeed + speedVariance);
                frogsList[selectedFrog].GetComponent<Frog>().Jump(speed);
            }
        }

        public int SelectFrog()
        {
            int selectedFrog = 0;
            do
            {
                selectedFrog = Random.Range(0, numberFrogs);
            } while (frogsList[selectedFrog].GetComponent<Frog>().IsJumping);

            return selectedFrog;
        }

        public int CountFrogs()
        {
            int cont = 0;
            foreach (var frog in frogsList)
            {
                Vector3 vec = new Vector3(0, 0, 0);
                if (frog.transform.position == vec) cont++;
            }
            return cont;
        }

        public int CompareUserInput(int userInput)
        {
            int correctNum = CountFrogs();
            if (userInput == correctNum)
                _gameManager.Score++;
            else
                _gameManager.ReduceLife();

            return correctNum;
        }
    }
}
