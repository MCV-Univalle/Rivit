using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controllengua : MonoBehaviour
{
    
    private Objectivo objectivo;
    private ControlScore control;
    private FrogController frog;
    // Start is called before the first frame update
    private void Awake() {
        objectivo = FindObjectOfType<Objectivo>();
        control = FindObjectOfType<ControlScore>();
        frog = FindObjectOfType<FrogController>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Negra" || other.gameObject.tag == "Cafe" || other.gameObject.tag == "Roja"
        ||other.gameObject.tag == "Blanca" || other.gameObject.tag == "Mariposa_dorada"){   
                if(!frog.notoco){
                    objectivo.se(other.gameObject.tag);
                    frog.notoco = true;
                    frog.move = false;
                }

        }
      
    }

}
