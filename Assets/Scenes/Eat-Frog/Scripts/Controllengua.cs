using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Eat_frog_Game
{
    public class Controllengua : MonoBehaviour
{
    
    private Objectivo objectivo;
    private FrogController frog;
    // Start is called before the first frame update
    private void Awake() {
        objectivo = FindObjectOfType<Objectivo>();
        frog = FindObjectOfType<FrogController>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    

}

}
