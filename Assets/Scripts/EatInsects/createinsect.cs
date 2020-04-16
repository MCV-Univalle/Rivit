 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createinsect : MonoBehaviour
{
    public GameObject[] bees;
    public GameObject frog;

    private ControlScore score;
    private int cont;
    
    public float tiempocreate = 2f, rango = 2f;

    void Awake() {
        score = FindObjectOfType<ControlScore>();
        
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
        if(frog.transform.position.y+1 <= spawsposition.y)
        {
            if(score.score >= 1500){
                GameObject insecto = Instantiate(bees[Random.Range(0,5)], transform.position, Quaternion.identity);
            }else
            {
                if(score.score >= 1500){
                    GameObject insecto = Instantiate(bees[Random.Range(0,4)], transform.position, Quaternion.identity);
                }else
                {
                    if(score.score >= 50){
                        GameObject insecto = Instantiate(bees[Random.Range(0,3)], spawsposition, Quaternion.identity);
                    }else{
                        GameObject insecto = Instantiate(bees[Random.Range(0,2)], spawsposition, Quaternion.identity);
                    }
                }
            }

           
        }
    }
   
}
