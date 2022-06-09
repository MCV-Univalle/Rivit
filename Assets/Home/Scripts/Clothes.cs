using UnityEngine;

public enum ClotheType
{
    Hat,
    Glasses,
    Accessory,
    Shirt
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Rivit/Clothe")]
public class Clothes : ScriptableObject
{
    public string clotheName;
    public Sprite sprite;
    public ClotheType type;
    public int price;
}