using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoColor : MonoBehaviour
{
    [SerializeField] private GameObject remo;

    [SerializeField] private ButtonsInsidePanelOutlineHandler handler;
    // Start is called before the first frame update
    void Start()
    {
        if(this.GetComponent<Image>().color == remo.transform.GetChild(1).gameObject.GetComponent<Image>().color)
            GetComponent<Outline>().enabled = true;
    }

    public void ChangeColor()
    {
        Color color = this.GetComponent<Image>().color;
        remo.transform.GetChild(1).gameObject.GetComponent<Image>().color = color; //Body
        remo.transform.GetChild(2).gameObject.GetComponent<Image>().color = color; //Head
        handler.DesactiveEveryOutline();
        GetComponent<Outline>().enabled = true;

    }
}
