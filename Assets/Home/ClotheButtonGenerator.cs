using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClotheButtonGenerator : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Clothes[] clothes;
    [SerializeField] private ClothesDataHandler dataHandler;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in clothes)
        {
            var temp = Instantiate(buttonPrefab);
            temp.GetComponent<ClotheButton>().Clothes = item;
            temp.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = item.sprite;
            if (dataHandler.Hat == item.clotheName || dataHandler.Glasses == item.clotheName || dataHandler.Accessory == item.clotheName || dataHandler.Shirt == item.clotheName)
                temp.GetComponent<Outline>().enabled = true;
            temp.transform.SetParent(this.transform, false);
        }
    }
}
