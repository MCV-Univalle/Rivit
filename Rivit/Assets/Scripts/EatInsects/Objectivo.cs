using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objectivo : MonoBehaviour
{

    public GameObject[] Insects;
    private GameObject insecto;
    private FrogController frog;

    public string objet;
    
    private int numb;

    public int Resistencia;

    private ControlScore control;
    public int Score;
    // Start is called before the first frame update
     void Awake() {
        control = FindObjectOfType<ControlScore>();
        
        
    }
    void Start()
    {
        frog = FindObjectOfType<FrogController>();
        insecto = GetComponent<GameObject>();
        numb = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(numb == 0)
        {
            if(control.score >= 200){
                 insecto = Instantiate(Insects[Random.Range(0,5)], transform.position, Quaternion.identity);

            }else
            {
                    if(control.score >= 150){
                    insecto = Instantiate(Insects[Random.Range(0,4)], transform.position, Quaternion.identity);
                }else
                {
                    if(control.score >= 50)
                    {
                        insecto = Instantiate(Insects[Random.Range(0,3)], transform.position, Quaternion.identity);
                    }else
                    {
                            insecto = Instantiate(Insects[Random.Range(0,2)], transform.position, Quaternion.identity);
                    }

                }
            }

            
           
            numb = 1;
        }
        
    }

    public void se(string ds){
        if(ds == insecto.gameObject.tag && frog.con == 0){

            if(insecto.gameObject.tag == "Blanca"){
                if(Resistencia == 3){
                    frog.move = false;
                    frog.curhealth += 20f;
                    frog.healthlive.fillAmount =frog.curhealth/frog.maxhealth;
                    control.score +=20;
                    Destroy(insecto);
                    numb = 0;
                }
            }
            
            if(insecto.gameObject.tag == "Roja"){
                if(Resistencia == 2){
                    frog.move = false;
                    frog.curhealth += 15f;
                    frog.healthlive.fillAmount =frog.curhealth/frog.maxhealth;
                    control.score +=15;
                    Destroy(insecto);   
                    numb = 0;
                }
            }
            if( insecto.gameObject.tag == "Mariposa_dorada"){
                frog.move = false;
                frog.curhealth += 25f;
                frog.healthlive.fillAmount =frog.curhealth/frog.maxhealth;
                control.score +=25;
                Destroy(insecto);
                numb = 0;

            }

            if(insecto.gameObject.tag == "Negra"|| insecto.gameObject.tag =="Cafe")
            {
                frog.move = false;
                frog.curhealth += 10f;
                frog.healthlive.fillAmount =frog.curhealth/frog.maxhealth;
                control.score +=10;
                Destroy(insecto);
                numb = 0;
            }
           
        }
    }

}
