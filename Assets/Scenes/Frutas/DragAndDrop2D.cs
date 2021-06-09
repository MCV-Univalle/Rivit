using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop2D : MonoBehaviour
{
    private bool dragging;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = this.gameObject.transform.position;
    }

    public void OnMouseDown()
    {
        dragging = true;
    }

    public void OnMouseUp()
    {
        dragging = false;
        this.gameObject.transform.position = initialPosition;
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
