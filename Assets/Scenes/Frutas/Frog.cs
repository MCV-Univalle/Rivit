using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fruits
{
    public delegate void Notify(int num);
    public class Frog : MonoBehaviour
    {
        public float ActiveTime { get; set; }
        private float remainingTime;
        public FruitType DesiredFruit { get; set; }
        public FruitSpawner TargetedSpanwer { get => targetedSpanwer; set => targetedSpanwer = value; }

        public static event Notify onScore;

        private FruitSpawner targetedSpanwer;

        private void Update()
        {
            if(remainingTime > 0)
                remainingTime -= Time.deltaTime;
        }

        public void WaitAndDestroy(float time)
        {
            ActiveTime = time;
            remainingTime = time;
            LeanTween.delayedCall(gameObject, time, () => Destroy(this.gameObject));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.transform.tag == "Fruit")
            {
                if (collision.GetComponent<Fruit>().Type == DesiredFruit)
                {
                    if (remainingTime > ActiveTime / 2F)
                        onScore(3);
                    else onScore(1);
                    Destroy(collision.gameObject);
                    Destroy(this.gameObject);
                }
                else
                {
                    Destroy(collision.gameObject);
                    Destroy(this.gameObject);
                }
                    
            }
        }
    }
}