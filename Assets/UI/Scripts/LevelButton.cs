using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{

    private int id;
    public int Id
    {
        get => id;
        set
        {
            id = value;
            this.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = id + "";
        }
    }
    public bool IsActive { get; set; }

    public void SelectButton()
    {
        var levelSelectionScript = gameObject.transform.parent.parent.GetComponent<LevelSelectionScreen>();
        levelSelectionScript.DeselectEveryButton();
        this.gameObject.GetComponent<Image>().color = new Color(1F, 1F, 1F, 1F);
        IsActive = true;
        levelSelectionScript.ActiveButton = this.gameObject;
        levelSelectionScript.SelectLevel();
    }
}
