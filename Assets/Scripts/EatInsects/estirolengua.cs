using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class estirolengua : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private float lineWhit = 0.15f;


    // Start is called before the first frame update
    void Start()
    {

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWhit;
        lineRenderer.enabled = false;

        
    }

    public void RenderLine(Vector3 endposition, bool enablerender){
        if(enablerender){
            if(!lineRenderer.enabled){
                lineRenderer.enabled = true;
            }
        } else {
            if(lineRenderer.enabled){
                lineRenderer.enabled = false;
            }else{
                 lineRenderer.enabled = true;
            }
        }


        if(lineRenderer.enabled){
            lineRenderer.sortingOrder = 4;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0,transform.position);
            lineRenderer.SetPosition(1,endposition);
            if(endposition.y <= transform.position.y){
                
                lineRenderer.sortingOrder = 0;
                lineRenderer.positionCount = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
