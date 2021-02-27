using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Eat_frog_Game
{
    public class Controllengua : MonoBehaviour
{
    
    private Objective Objective;
    private FrogController frog;
    // Start is called before the first frame update
    private void Awake() {
        Objective = FindObjectOfType<Objective>();
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
