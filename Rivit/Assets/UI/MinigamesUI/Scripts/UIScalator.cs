using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScalator : MonoBehaviour
{
    [SerializeField]
    private float _targetScale;
    private bool _pressed = false;
    // Start is called before the first frame update
    void OnEnable()
    {
        transform.localScale = new Vector3(1, 1, 1);
        gameObject.GetComponent<Button>().interactable = true;
    }

    public void ChangeScale(bool value)
    {
        if(value)
        StartCoroutine(Contract());
        else 
        StartCoroutine(Expand());
    }

    public IEnumerator Contract()
    {
        _pressed = true;
        while(((transform.localScale.x > _targetScale) && _pressed))
        {
            transform.localScale -= new Vector3(0.01F, 0.01F, 0);
            yield return null;
        }
    }

    public void LockInteractable()
    {
        gameObject.GetComponent<Button>().interactable = false; // The boton is locked so it can't be pressed twice while it is scaling
    }

    public IEnumerator Expand()
    {
        _pressed = false;
        while(((transform.localScale.x < 1) && !_pressed))
        {
            transform.localScale += new Vector3(0.01F, 0.01F, 0);
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
