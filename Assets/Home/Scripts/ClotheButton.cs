using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void Notify(Clothes clothes);
public class ClotheButton : MonoBehaviour
{
    [SerializeField] private Clothes clothes;
    public PurchaseHandler Handler { get; set; }
    public bool AlreadyPurchased { get; set; }

    public Clothes Clothes { get => clothes; set => clothes = value; }

    public static event Notify changeClothes;
    public void OnPressed()
    {
        if (AlreadyPurchased)
        {
            changeClothes?.Invoke(Clothes);
            this.GetComponent<Outline>().enabled = true;
        }
        else
            Handler.OpenPanel(this.gameObject);
    }
}
