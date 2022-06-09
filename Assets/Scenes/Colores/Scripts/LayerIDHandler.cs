using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 public class LayerIDHandler : MonoBehaviour
 {

    void Start ()
 {
     this.GetComponent<Renderer>().sortingLayerID =
     this.transform.parent.GetComponent<Renderer>().sortingLayerID;
 }
 }