using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fruits
{
    [System.Serializable]
    public enum FruitType
    {
        Apple,
        Pineapple,
        Pear,
        Grapes,
        Strawberry,
        Cherry,
        Banana,
        Peach,
        Lemon,
        Orange,
        Kiwi,
        Watermelon
    }

    public class Fruit : MonoBehaviour
    {
        FruitType type;

        public FruitType Type { get => type; set => type = value; }
    }
}

