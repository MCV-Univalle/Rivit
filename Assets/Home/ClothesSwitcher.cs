using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClothesSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject hat;
    [SerializeField] private GameObject glasses;
    [SerializeField] private GameObject accessory;
    [SerializeField] private GameObject shirt;

    private ClothesDataHandler dataHandler;

    [SerializeField] private StringSpriteDictionary clothesDictionary;
    private void Awake()
    {
        dataHandler = this.GetComponent<ClothesDataHandler>();
    }
    void Start()
    {
        ClotheButton.changeClothes += ChangeClothes;
        
    }

    private void OnDestroy()
    {
        ClotheButton.changeClothes -= ChangeClothes;
    }

    public void ChangeClothes(Clothes clothes)
    {
        ClotheType type = clothes.type;
        string name = clothes.name;
        ChangeClothes(type, name);
    }

    public void ChangeClothes(ClotheType type, string name)
    {
        switch (type)
        {
            case ClotheType.Hat:
                hat.GetComponent<Image>().sprite = clothesDictionary[name];
                dataHandler.Hat = name;
                break;
            case ClotheType.Glasses:
                glasses.GetComponent<Image>().sprite = clothesDictionary[name];
                dataHandler.Glasses = name;
                break;
            case ClotheType.Accessory:
                accessory.GetComponent<Image>().sprite = clothesDictionary[name];
                dataHandler.Accessory = name;
                break;
            case ClotheType.Shirt:
                shirt.GetComponent<Image>().sprite = clothesDictionary[name];
                dataHandler.Shirt = name;
                break;
        }
    }
}
