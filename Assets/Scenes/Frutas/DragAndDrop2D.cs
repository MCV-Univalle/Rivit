using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop2D : MonoBehaviour
{
    private bool dragging;
    private Vector3 initialPosition;

    public AudioManager SFXManager { get; set; }

    private void Start()
    {
        initialPosition = this.gameObject.transform.position;
    }

    public void Drag()
    {
        dragging = true;
        SFXManager.PlayAudio("Pop");
    }

    public void OnMouseDown()
    {
        Drag();
    }

    public void Drope()
    {
        dragging = false;
        this.gameObject.transform.position = initialPosition;
        SFXManager.PlayAudio("Pop");
    }

    public void OnMouseUp()
    {
        if(dragging)
            Drope();
    }
    void Update()
    {
        if (dragging)
            Move();
    }
    public void Move()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y)) - this.transform.position;

         transform.Translate(mousePosition);
    }
}
