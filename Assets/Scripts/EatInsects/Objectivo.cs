using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  Eat_frog_Game
{
    public class Objectivo : MonoBehaviour
{

    public GameObject[] Insects;
    public  GameObject insecto,soundfallo,soundcorrect;
    private FrogController frog;

    public bool fallo;

    public string objet;
    
    private int numb;

    public int Resistencia;
    // Start is called before the first frame update

    void Awake()
    {
        frog = FindObjectOfType<FrogController>();
    }

    void Start()
    {
        insecto = GetComponent<GameObject>();
        numb = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.Instance.Active){
            Destroy(insecto);
            numb =0;
        }   
        //if(GameManager.Instance.limitreached){
          //  GameManager.Instance.limitreached = false;
          //  frog.curhealth -= 10f;
          //  frog.healthlive.fillAmount =frog.curhealth/frog.maxhealth;
            
        //}
        if(numb == 0 && GameManager.Instance.Active)
        {
            if(GameManager.Instance.Score >= 1500){
                 insecto = Instantiate(Insects[Random.Range(0,5)], transform.position, Quaternion.identity);

            }else
            {
                    if(GameManager.Instance.Score >= 1500){
                    insecto = Instantiate(Insects[Random.Range(0,4)], transform.position, Quaternion.identity);
                }else
                {
                    if(GameManager.Instance.Score >= 50)
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

    public void se(string ds,bool limitreached){
        if(!limitreached){
            if(ds == insecto.gameObject.tag){
                if(insecto.gameObject.tag == "Blanca"){
                        frog.move = false;
                        frog.curhealth += 20f;
                        frog.healthlive.fillAmount =frog.curhealth/frog.maxhealth;
                        GameManager.Instance.Score +=20;
                        Destroy(insecto);
                        numb = 0;
                }
                
                if(insecto.gameObject.tag == "Roja"){
                        frog.move = false;
                        frog.curhealth += 15f;
                        frog.healthlive.fillAmount =frog.curhealth/frog.maxhealth;
                        GameManager.Instance.Score +=15;
                        Destroy(insecto);   
                        numb = 0;
                }
                if( insecto.gameObject.tag == "Mariposa_dorada"){
                    frog.move = false;
                    frog.curhealth += 25f;
                    frog.healthlive.fillAmount =frog.curhealth/frog.maxhealth;
                    GameManager.Instance.Score +=25;
                    Instantiate(soundcorrect); 
                    Destroy(insecto);
                    numb = 0;

                }

                if(insecto.gameObject.tag == "Negra"|| insecto.gameObject.tag =="Cafe")
                {
                    frog.curhealth += 10f;
                    frog.healthlive.fillAmount =frog.curhealth/frog.maxhealth;
                    GameManager.Instance.Score +=10;
                    Instantiate(soundcorrect); 
                    Destroy(insecto);
                    numb = 0;
                }
                
            }else{
                Instantiate(soundfallo); 
                frog.animator.SetBool("fallo",true);
                frog.curhealth -= 2f;
                fallo = true;
                frog.healthlive.fillAmount =frog.curhealth/frog.maxhealth;
                Destroy(insecto);
                numb = 0;
            }
        }
        
    }

}

}

