using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fruits
{
    [System.Serializable]
    public class FruitsPickerAdditionalData
    {
        public int RigthAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public float AverageTime { get; set; }
    }
}