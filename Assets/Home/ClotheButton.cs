using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void Notify(Clothes clothes);
public class ClotheButton : MonoBehaviour
{
    [SerializeField] private Clothes clothes;

    public Clothes Clothes { get => clothes; set => clothes = value; }

    public static event Notify changeClothes;

    public void OnPressed()
    {
        changeClothes(Clothes);
        this.GetComponent<Outline>().enabled = true;
    }
}
