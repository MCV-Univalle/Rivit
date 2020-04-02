using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class nenufar : MonoBehaviour

{

    #region de singleton
    public static nenufar instance;

    private void Awake()
    {
        Application.targetFrameRate = 30;
        instance = this;
    }

    #endregion

    public static nenufar Instance()
    {
        if (!instance)
        {
            instance = FindObjectOfType(typeof(nenufar)) as nenufar;
            if (!instance)
                Debug.LogError("error nenufar");
        }

        return instance;
    }

    public void gameOver(float positionX, float positionY, GameObject frog, GameObject leaf, Color color, float timeFrog, bool ranaPresente){

        if ((frog.GetComponent<Transform>().position.x == positionX) && (frog.GetComponent<Transform>().position.y == positionY))
        {
                leaf.GetComponent<SpriteRenderer>().color = color;           
        }

    }

    public void keppPlaying(float positionsX, float positionsY, GameObject frog, Color hojaNormal, GameObject leaf)
    {

        if ((frog.GetComponent<Transform>().position.x == positionsX) && (frog.GetComponent<Transform>().position.y == positionsY))
        {
            leaf.GetComponent<SpriteRenderer>().color = hojaNormal;
        }


    }

    public GameObject item;
    public float posLeafX;
    public float posLeafY;

    // Start is called before the first frame update
    void Start()
    {

    }

    

    void Update() {

        
    }
}