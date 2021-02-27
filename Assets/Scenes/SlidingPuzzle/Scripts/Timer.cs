using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float CurrentTime { get; set; }
    public bool Started { get; set; }
    [SerializeField] TextMeshProUGUI timeText;

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Update is called once per frame
    void Update()
    {
        if(Started)
            CurrentTime += Time.deltaTime;
        DisplayTime(CurrentTime);
    }
}
