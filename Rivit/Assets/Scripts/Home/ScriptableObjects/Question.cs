using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Home
{
    [System.Serializable]

    public struct Choice
    {
        public string text;
        public Dialogue dialogue;
    }

    [CreateAssetMenu(fileName = "New Question", menuName = "ScriptableObjects/Conversation/Question")]
    public class Question : ScriptableObject
    {
        public string sentence;
        public Choice[] choices;
    }   
}