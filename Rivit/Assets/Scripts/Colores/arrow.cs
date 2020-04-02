using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrow : MonoBehaviour
{

    #region de singleton
    public static arrow instance;

    private void Awake()
    {
        Application.targetFrameRate = 30;
        instance = this;
        rBody = GetComponent<Rigidbody2D>();
    }

    #endregion

    public static arrow Instance()
    {
        if (!instance)
        {
            instance = FindObjectOfType(typeof(arrow)) as arrow;
            if (!instance)
                Debug.LogError("error arrow");
        }

        return instance;
    }

    private float longitud;
    private Rigidbody2D rBody;
    public float speed = -1.5f;


    // Start is called before the first frame update
    void Start()
    {
        longitud = 43;
        rBody.velocity = new Vector2(speed, 0);

    }


    // Update is called once per frame
    void Update()
    {
      


    }
}
