using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class jump : MonoBehaviour
{
    
    #region de singleton
    public static jump instance;

    private void Awake()
    {
        Application.targetFrameRate = 30;
        instance = this;
    }

    #endregion

    public static jump Instance()
    {
        if (!instance)
        {
            instance = FindObjectOfType(typeof(jump)) as jump;
            if (!instance)
                Debug.LogError("error jump");
        }

        return instance;
    }

    

    public static GameObject itemDragging;
    Vector3 newPos;
    Transform startParent;
    Transform dragParent;
    public float z = 0.0f;

    public Vector2 initialPos;
    public Vector2 mousePos;
    public float deltax, deltay;

    public float posLeafX1;
    public float posLeafY1;
    public float posLeafX2;
    public float posLeafY2;
    public float posLeafX3;
    public float posLeafY3;
    public float posLeafX4;
    public float posLeafY4;
    public float posLeafX5;
    public float posLeafY5;
    public float posLeafX6;
    public float posLeafY6;
    public float posLeafX7;
    public float posLeafY7;
    public float posLeafX8;
    public float posLeafY8;
    public float posLeafX9;
    public float posLeafY9;

    void Start()
    {
        
        newPos = new Vector3(40f, 35.985f, 0f);
       
    }
    
    


}
