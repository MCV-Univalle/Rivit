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
        public string additionalInstruction;
    }

    [CreateAssetMenu(fileName = "New Dialogue", menuName = "ScriptableObjects/Rivit/Conversation/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        public LineText[] sentences;
    }
}