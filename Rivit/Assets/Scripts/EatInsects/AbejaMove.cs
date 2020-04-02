using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbejaMove : MonoBehaviour
{
    // Start is called before the first frame updat
    private int ZigZagMove,TypeMove;
    public int Resistencia;

    public float Timee;
    public float vel;
    private Objectivo objectivo;
    private ControlScore score;
    private FrogController frog;
    private bool Change_Direction,Invisiblee;

    void Awake() {
        score = FindObjectOfType<ControlScore>();
        objectivo = FindObjectOfType<Objectivo>();
        frog = FindObjectOfType<FrogController>();
        
    }

    void Start()
    {
        Change_Direction = true;
        Timee = 0f;
        Invisiblee = false;
        ZigZagMove = 0;
        Resistencia = 0;
        vel = 1.25f;
        TypeMove  = Random.Range(1,3);
    }

    // Update is called once per frame
    void Update()
    {
        //upvel(control.score);
        move();
        velTypeBee();
        special();
    }

    void special(){
        if(gameObject.tag == "Mariposa_dorada"){
            Timee +=  Time.deltaTime;
            if(!Invisiblee){
                if(Timee >= Random.Range(1,2)){
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = -1;
                    Timee = 0;
                    Invisiblee = true;
                }
            }else{
                if(Timee >= Random.Range(1,2)){
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    Timee = 0;
                    Invisiblee = false;
                }
            }

            
        }
    }
    /**
    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(5);
        if(Invisiblee){
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = -1;
           
            Invisiblee = false;
        }else{
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
            Invisiblee = true;
        }
    }
*/


    void velTypeBee(){
        if(gameObject.tag == "Roja"){
            vel = 0.9f;
        }
        if(gameObject.tag == "Blanca"){
            vel = 0.8f;
        }
    }

    void aumVel(){
        if(score.score >= 150){
            vel+=0.5f;
        }
    }

    void move(){
        switch(TypeMove){
                case 1:
                    transform.position += transform.right * vel * Time.deltaTime;
                    
                        if(Change_Direction){
                            transform.position += transform.up * vel * Time.deltaTime;
                            ZigZagMove +=1;
                            if(ZigZagMove ==50){
                                Change_Direction= false;
                        }
                            }else{
                                transform.position -= transform.up * vel * Time.deltaTime;
                                ZigZagMove -=1;
                                if(ZigZagMove == 0){
                                     Change_Direction= true;;
                                    }
                            }
                    break;
                case 2:
                        transform.position += transform.right * vel * Time.deltaTime;
                    break;
            }

    }

    void OnTriggerEnter2D(Collider2D other) {

        if(other.gameObject.tag == "lengua")
        {
            if(!frog.notoco){
            if(gameObject.tag == "Blanca"){
                Resistencia++;
                print(Resistencia);
                objectivo.Resistencia = Resistencia;
                if(Resistencia == 3){
                    Destroy(gameObject);
                }
            }
            
            if(gameObject.tag == "Roja"){
                Resistencia++;
                objectivo.Resistencia = Resistencia;
                if(Resistencia == 2){
                    Destroy(gameObject);
                }
            }

            if(gameObject.tag == "Negra" || gameObject.tag =="Cafe" || gameObject.tag == "Mariposa_dorada")
            {
               Destroy(gameObject);
            }
            }
        }
        if(other.gameObject.tag ==  "Destroyer")
        {
        
            print(Resistencia);
            Destroy(gameObject);   
        }

        }
}
