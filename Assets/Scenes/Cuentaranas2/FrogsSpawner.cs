using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorsFrogCounter
{
    public class FrogsSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _frogPrefab;
        [SerializeField] private List<Color> _colorList;
        public int MaxNumberOfFrogs { get; set; }
        public int MaxColors { get; set; }
        public int[] NumberOfFrogsForEachCholor { get; set; }

        public int Total { get; set; }
        public List<Color> ColorList { get => _colorList; set => _colorList = value; }

        private IEnumerator spawnCoroutine;
        public void StartSpawning()
        {
            spawnCoroutine = SpawnFrog(0.1F);
            StartCoroutine(spawnCoroutine);
        }

        public void StopSpawning()
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        public void DestroyFrogs()
        {
            foreach (Transform item in transform)
            {
                Destroy(item.gameObject);
            }
        }



        private IEnumerator SpawnFrog(float waitTime)
        {
            int[] numberOfFrogs = new int[MaxColors];
            
            int total = 0;
            for (int i = 0; i < MaxColors; i++)
            {
                numberOfFrogs[i] = Random.Range(1, MaxNumberOfFrogs);
                total += numberOfFrogs[i];
            }
            NumberOfFrogsForEachCholor = (int[]) numberOfFrogs.Clone();
            

            Total = total;
            for (int i = 0; i < total; i++)
            {
                int colorIndex = Random.Range(0, MaxColors);
                while(numberOfFrogs[colorIndex] == 0)
                {
                    colorIndex = Random.Range(0, MaxColors);
                }
                Frog go = Instantiate(_frogPrefab, transform).GetComponent<Frog>();
                go.AppearInRandomPosition();
                yield return new WaitForSeconds(waitTime);
                go.FixedPosition = true;
                go.transform.GetChild(0).gameObject.GetComponent<Animator>().SetTrigger("Appear");
                go.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = _colorList[colorIndex];
                numberOfFrogs[colorIndex]--;
            }
        }
    }
}