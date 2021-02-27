using UnityEngine;

public class CameraSize : MonoBehaviour
{
    [SerializeField] private float sizeCamera;

    private void Start()
    {
        sizeCamera = (float) Screen.height / Screen.width;
        
    }


    void Update()
    {
        sizeCamera = (float)Screen.height / Screen.width;
        Camera.main.orthographicSize = sizeCamera;
    }
}
