 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Eat_frog_Game
{
    public class createinsect : MonoBehaviour
{
    public GameObject[] bees;
    public GameObject frog;


    private int cont;
    
    public float tiempocreate = 2f, rango = 2f;

    void Awake() {
        
        
    }
    // Start is called before the first frame update

    void Start()
    {   
        InvokeRepeating("creando",0.0f,tiempocreate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void repit(){
    }

    public void creando(){
        Vector3 spawsposition = new Vector3(0,0,0);
        spawsposition = this.transform.position + Random.onUnitSphere * rango;
        spawsposition = new Vector3(this.transform.position.x,spawsposition.y,0);
        List<int> numberinsects = new List<int>{};
        Random ra = new Random();
        if(frog.transform.position.y+1 <= spawsposition.y && GameManager.Instance.Active)
        {
            if(GameManager.Instance.Score >= 1500){
                GameObject insecto = Instantiate(bees[Random.Range(0,5)], transform.position, Quaternion.identity);
            }else
            {
                if(GameManager.Instance.Score >= 1500){
                    GameObject insecto = Instantiate(bees[Random.Range(0,4)], transform.position, Quaternion.identity);
                }else
                {
                    if(GameManager.Instance.Score >= 50){
                        numberinsects.Clear();
                       if(numberinsects.Count==0){
                            numberinsects.Add(0);
                            numberinsects.Add(1);
                            numberinsects.Add(2);
                        }
                        int numb = Random.Range(0,numberinsects.Count);
                        GameObject insecto = Instantiate(bees[numb], spawsposition, Quaternion.identity);
                        numberinsects.Remove(numberinsects[numb]);   

                        
                    }else{
                        if(numberinsects.Count==0){
                            numberinsects.Add(0);
                            numberinsects.Add(1);
                        }
                        int numb = Random.Range(0,numberinsects.Count);
                        GameObject insecto = Instantiate(bees[numb], spawsposition, Quaternion.identity);
                        numberinsects.Remove(numberinsects[numb]);   
                    }
                }
            }

           
        }
    }
   
}

}
