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
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject desireBubble;
        [SerializeField] private GameObject fruit;
        [SerializeField] private ParticleSystem particleSystem;

        private IEnumerator currentCoroutine;

        public AudioManager SFXManager { get; set; }
        public GameObject Fruit { get => fruit; set => fruit = value; }

        public FruitsPickerGameManager FruitsGameManager { get; set; }

        public bool Finished { get; set; }
        public float SelfDestructionTime { get; set; }

        private void Update()
        {
            if(remainingTime > 0)
                remainingTime -= Time.deltaTime;
        }

        private void Autodestroy()
        {
            var go = this.gameObject.transform.parent.gameObject;
            LeanTween.scale(go, new Vector3(0, 0, 0), 0.05F);
            LeanTween.moveY(go, go.transform.position.y - 0.5F, 0.05F);
            LeanTween.delayedCall(gameObject, 0.05F, () => Destroy(go));
        }

        private void Start()
        {
            GameManager.endGame += Autodestroy;
            currentCoroutine = WaitAndDestroy();
            StartCoroutine(currentCoroutine);
        }

        private void OnDestroy()
        {

            GameManager.endGame -= Autodestroy;
        }
        private void Alert()
        {
            float pos = desireBubble.transform.position.y;
            LeanTween.moveY(desireBubble.gameObject, pos + 0.15F, 0.2F).setLoopPingPong();
        }
        public IEnumerator WaitAndDestroy()
        {
            ActiveTime = SelfDestructionTime;
            remainingTime = SelfDestructionTime;
            yield return new WaitForSeconds(SelfDestructionTime * 0.6F);
            Alert();
            yield return new WaitForSeconds(SelfDestructionTime * 0.4F);
            Autodestroy();
        }

        private void PositiveFeedBack()
        {
            FruitsGameManager.AdditionalData.RigthAnswers++;
            FruitsGameManager.TotalTime += ActiveTime - remainingTime;
            animator.SetBool("Correct", true);
            SFXManager.PlayAudio("Boing2");
            float pos = transform.position.y;
            LeanTween.moveY(this.gameObject, pos + 0.4F, 0.15F).setLoopPingPong(1);
            LeanTween.alpha(desireBubble, 0, 0.2F);
            LeanTween.scale(desireBubble, new Vector3(0, 0, 0), 0.2F);
            LeanTween.delayedCall(this.gameObject, 1F, () => Autodestroy());
        }

        private void NegativeFeedBack()
        {
            FruitsGameManager.AdditionalData.WrongAnswers++;
            FruitsGameManager.TotalTime += ActiveTime - remainingTime;
            animator.SetBool("Wrong", true);
            SFXManager.PlayAudio("Wrong");
            LeanTween.alpha(desireBubble, 0, 0.2F);
            LeanTween.scale(desireBubble, new Vector3(0, 0, 0), 0.2F);
            LeanTween.delayedCall(this.gameObject, 1F, () => Autodestroy());
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.transform.tag == "Fruit")
            {
                StopCoroutine(currentCoroutine);
                Finished = true;
                if (collision.GetComponent<Fruit>().Type == DesiredFruit)
                {
                    if (remainingTime > ActiveTime / 2F)
                    {
                        onScore(3);
                    }
                        
                    else onScore(1);
                    Destroy(collision.gameObject);
                    PositiveFeedBack();
                    particleSystem.gameObject.SetActive(true);
                }
                else
                {
                    collision.gameObject.GetComponent<DragAndDrop2D>().Drope();
                    NegativeFeedBack();
                }
                    
            }
        }
    }
}