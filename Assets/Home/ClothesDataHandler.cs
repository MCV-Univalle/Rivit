using Sisus.Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[System.Serializable]
public class ClothesData
{
    public string hat;
    public string glasses;
    public string accessory;
    public string shirt;
    public string color;
}

public class ClothesDataHandler : MonoBehaviour
{
    public string Hat { get; set; }
    public string Glasses { get; set; }
    public string Accessory { get; set; }
    public string Shirt { get; set; }

    [Inject] private UserDataManager userDataManager;

    public void SaveData()
    {
        ClothesData data = new ClothesData();
        data.hat = Hat;
        data.glasses = Glasses;
        data.accessory = Accessory;
        data.shirt = Shirt;
        string colorHTML = ColorUtility.ToHtmlStringRGB(this.transform.GetChild(1).gameObject.GetComponent<Image>().color);
        data.color = "#" + colorHTML;
        string json = JsonConvert.SerializeObject(data);
        userDataManager.SaveClothes(json);
        SceneSwitcher.GoToHomeScreen();
    }

    private void LoadData()
    {
        string jsonString = userDataManager.LoadClothes();
        if (jsonString != "")
        {
            var temp = GetComponent<ClothesSwitcher>();
            ClothesData data = new ClothesData();
            data = JsonConvert.DeserializeObject<ClothesData>(jsonString);
            temp.ChangeClothes(ClotheType.Hat, data.hat);
            temp.ChangeClothes(ClotheType.Glasses, data.glasses);
            temp.ChangeClothes(ClotheType.Accessory, data.accessory);
            temp.ChangeClothes(ClotheType.Shirt, data.shirt);
            Color color;
            if (ColorUtility.TryParseHtmlString(data.color, out color))
            {
                this.transform.GetChild(1).gameObject.GetComponent<Image>().color = color;
                this.transform.GetChild(2).gameObject.GetComponent<Image>().color = color;
            }
        }
        else
        {
            Debug.LogWarning("Save data not found!");
        }
    }

    void Start()
    {
        LoadData();
    }
}
