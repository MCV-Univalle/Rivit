using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlScore : MonoBehaviour
{
    // Start is called before the first frame update
    
    public int score;
    private string ScoreString = "Score ";

    public Text TextScore;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(TextScore != null){
            TextScore.text =  ScoreString + score.ToString();
        }
        
    }
}
