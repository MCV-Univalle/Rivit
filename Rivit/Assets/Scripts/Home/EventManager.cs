using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    private Dictionary <string, UnityEvent> eventDictionary = new Dictionary<string, UnityEvent>();

    private static EventManager _instance;

    public static EventManager Instance
    {
        get
        {
            //Logic to create the instance
            if(_instance = null)
            {
                _instance = new EventManager();
            }
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
        Init();
    }

    void Init()
    {
        if(eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    public void StartListening (string eventName, UnityAction listener)
    {
        UnityEvent evnt = null;
        if(eventDictionary.TryGetValue (eventName, out evnt))
        {
            evnt.AddListener(listener);
        }
        else
        {
            evnt = new UnityEvent();
            evnt.AddListener(listener);
            eventDictionary.Add(eventName, evnt);
        }
    }

    public void StopListening (string eventName, UnityAction listener)
    {
        UnityEvent evnt = null;
        if(eventDictionary.TryGetValue (eventName, out evnt))
        {
            evnt.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName)
    {
        UnityEvent evnt = null;
        if(eventDictionary.TryGetValue (eventName, out evnt))
        {
            evnt.Invoke();
        }
        else Debug.Log("Event not found");
    }
}
