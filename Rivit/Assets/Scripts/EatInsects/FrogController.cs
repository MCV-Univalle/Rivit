using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrogController : MonoBehaviour
{
    // Start is called before the first frame update
   
    public float distancia = 10f;
    public GameObject camaraover,live,liveup;

    public float maxhealth = 100f;
    public float curhealth;
    private ControlScore control;


    public Image healthlive, healthliveup;
    
    public GameObject len;
    private bool lol,ll;
    public bool notoco,LiveMAx,move;
    private float angle;
    public int vel_lengua;
    public int con;
    public string obe;
    private Vector3 lookPos;

    private int Cont;
    private BoxCollider2D box;
    private Vector3 lengua;
    private Rigidbody2D rd2;
    private LayerMask mask;
    private Animator animator;
    private SpriteRenderer sprite;

    private estirolengua estiro;

    public Canvas canvas;
    


     void Awake()
    {
        estiro = GetComponent<estirolengua>();
        control = FindObjectOfType<ControlScore>();
    }

    void Start()
    
    {
        
        sprite = GetComponent<SpriteRenderer>();
        rd2 = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("Volver",false);
        curhealth = maxhealth;
        healthlive.fillAmount = curhealth/maxhealth;
        notoco = false;
        LiveMAx = false;
        //lineRenderer.enabled = false;  
        lol = true;
        vel_lengua = 20;
    }

    // Update is called once per frame
    void Update()
    {
        Getinput();
        movelengua();
        healthmin();
        FrogHungry();
    }

    void FrogHungry(){
        if(healthlive.fillAmount == 0){
           camaraover.SetActive(true);
        }
    }

    void healthmin(){
        if(control.score >= 200){
            
            if((curhealth/maxhealth) > 1.1f){
                curhealth = 100f;
                healthlive.fillAmount = curhealth/maxhealth;
                return;
            }
            curhealth -= 0.4f;
            healthlive.fillAmount = curhealth/maxhealth;

        }else{
            
            if((curhealth/maxhealth) > 1.1f){
                curhealth = 100f;
                healthlive.fillAmount = curhealth/maxhealth;
                return;
            }
            curhealth -= 0.1f;
            healthlive.fillAmount = curhealth/maxhealth;
            }
    }


    public void Getinput(){
        if(Input.GetMouseButtonDown(0) || Input.GetKeyDown("up")){
            if(lol){
                lengua = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if(lengua.y <=(transform.position.y+1)){
                    return;
                }else{
                    UpdateState("frog-eat");
                    animator.SetBool("Volver",true);
                    lol = false;
                    move = true;    
                    Debug.DrawLine(transform.position,lengua,Color.blue);
                    len.transform.rotation = Quaternion.LookRotation(Vector3.forward, lengua - len.transform.position);
                    
                }
              
                //RaycastHit2D hit = Physics2D.Raycast(transform.position, lengua - transform.position,distancia,mask);
                //transform.Translate(0,0, 10*Time.deltaTime);
            }
        }
    }
    private void movelengua(){
        if(lol){
            return;
        }

        if(!lol){
            Vector3 temp = len.transform.position;
            Vector3 temp2 = len.transform.position;
            estiro.RenderLine(temp, false);
            len.GetComponent<SpriteRenderer>().sortingOrder = 2;
            if(move){
                temp += len.transform.up * Time.deltaTime *vel_lengua;
            }else{
                temp -= len.transform.up * Time.deltaTime *vel_lengua;
                
            }

            len.transform.position = temp;

            if(temp.y >= lengua.y){
                move = false;
                notoco = true;
            }
            if(temp.y <= transform.position.y){
                len.transform.position=transform.position;
                animator.SetBool("Volver",false);
                len.GetComponent<SpriteRenderer>().sortingOrder = 0;
                lol = true;
                notoco = false;
            }
            estiro.RenderLine(temp, true);
        }
    }
    

    public void UpdateState(string state = null){
        if(state!=null){
            animator.Play(state);
        }
    }


}
