using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Eat_frog_Game
{
    public class estirolengua : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private float lineWhit = 0.15f;


    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startWidth = lineWhit; 
        _lineRenderer.enabled = false;
    }

    public void RenderLine(Vector3 endposition, bool enablerender, Transform startposicion){
        
        if(enablerender){
            if(!_lineRenderer.enabled){
                _lineRenderer.enabled = true;   
            }
        } else {
            if(_lineRenderer.enabled){
               _lineRenderer.enabled = false;
            }else{
                _lineRenderer.enabled = true;
            }
        }

        if(_lineRenderer.enabled){
            _lineRenderer.sortingOrder = 3;
            _lineRenderer.positionCount = 2;

            Vector3 temp = startposicion.position;
            temp.z = -1;
            startposicion.position = temp;

            _lineRenderer.SetPosition(0,startposicion.position);
            _lineRenderer.SetPosition(1,endposition);
            if(endposition.y <= startposicion.position.y){
                
                _lineRenderer.sortingOrder = 0;
                _lineRenderer.positionCount = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}

}
