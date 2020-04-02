using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Home
{
    [System.Serializable]
    public struct LineText
    {
        public string text;
        public string anim;
        public Action action;
    }

    [CreateAssetMenu(fileName = "New Dialogue", menuName = "ScriptableObjects/Conversation/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        public LineText[] sentences;
}   
}
