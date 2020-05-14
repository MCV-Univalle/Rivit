using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace  Eat_frog_Game
{
    public class FrogController : MonoBehaviour
{
    // Start is called before the first frame update
   
    public float distancia =     10f;
    public GameObject camaraover,live,liveup;

    public float maxhealth = 100f;
    public float curhealth;


    public Image healthlive, healthliveup;
    
    public GameObject len;
    private bool lol,ll;
    public bool notoco,LiveMAx,move;
    private float angle;
    public int vel_lengua;
    public string obe;
    private Vector3 lookPos;

    private int Cont;
    private BoxCollider2D box;
    private Vector3 lengua;
    private Rigidbody2D rd2;
    private LayerMask mask;
    public  Animator animator;
    private SpriteRenderer sprite;

    private estirolengua estiro;

    public Canvas canvas;

    private Transform _firepoint;

    public bool Tongue;
    
     void Awake()
    {
        _firepoint = transform.Find("FirePoint");
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
        Tongue = true;
        //lineRenderer.enabled = false;  
        lol = true;
        vel_lengua = 20;
         print(Tongue);
    }

    // Update is called once per frame
    void Update()
    {
        Getinput();
        healthmin();
        FrogHungry();
    }

    void FrogHungry(){
        if(healthlive.fillAmount == 0){
            GameManager.Instance.Active = false;
            Tongue = true;
            GameManager.Instance.paused = false;
            animator.SetBool("Volver",false);
            GameManager.Instance.ShowResults();
        }
    }

    void healthmin(){
        if(GameManager.Instance.Score >= 200){
            
            if((curhealth/maxhealth) > 1.1f){
                curhealth = 100f;
                healthlive.fillAmount = curhealth/maxhealth;
                return;
            }
            curhealth -= 0.4f;
            healthlive.fillAmount = curhealth/maxhealth;

        }else{
            if(!GameManager.Instance.paused){

                if(GameManager.Instance.Active){
                    live.SetActive(true);
                     
                if((curhealth/maxhealth) > 1.1f){
                    curhealth = 100f;
                    healthlive.fillAmount = curhealth/maxhealth;
                    return;
                }
                curhealth -= 0.1f;
                healthlive.fillAmount = curhealth/maxhealth;
                }else
                {
                    healthlive.fillAmount = 1;
                    live.SetActive(false);
                }
            }
            }
    }


    public void Getinput(){
        
        //Debug.Log(GameManager.Instance.paused);
        if(GameManager.Instance.Active){
            
            if(Input.GetMouseButtonDown(0) || Input.GetKeyDown("up")){
                Debug.Log(GameManager.Instance.paused+"Click");
                if(Tongue && !animator.GetBool("fallo"))
                {
                    lengua = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    UpdateState("frog-eat");
                    if(!GameManager.Instance.paused){
                        animator.SetBool("Volver",true);
                    }
                    Tongue = false;
                    Debug.DrawLine(transform.position,lengua,Color.blue);
                }
            }
            if(!GameManager.Instance.Active||GameManager.Instance.paused){
                Tongue = true;
                animator.SetBool("Volver",false);
            }
            
        }
            
           
    }

    

    public void UpdateState(string state = null){
        if(state!=null){
            animator.Play(state);
        }
    }

    public void cambio(){
        animator.SetBool("fallo",false);
    }

    
    public void CreateTongue(){
        GameObject tounge = Instantiate(len,_firepoint.transform.position,Quaternion.LookRotation(Vector3.forward,lengua - _firepoint.transform.position));
        TongueControl toungeComponent = tounge.GetComponent<TongueControl>();
        toungeComponent.start = _firepoint.transform.position.y;   
        toungeComponent.limit = lengua.y;   
        toungeComponent._firepoint = _firepoint;
        if(GameManager.Instance.paused){
            Destroy(tounge);
            animator.SetBool("Volver",false);
        }
    }


}

}
