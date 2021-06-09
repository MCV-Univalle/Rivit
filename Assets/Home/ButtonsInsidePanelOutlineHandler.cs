using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsInsidePanelOutlineHandler : MonoBehaviour
{
    private void OnEnable()
    {
        ClotheButton.changeClothes += DesactiveEveryOutline;
    }

    private void OnDisable()
    {
        ClotheButton.changeClothes -= DesactiveEveryOutline;
    }

    public void DesactiveEveryOutline(Clothes clothes)
    {
        foreach (Transform item in transform)
        {
            item.gameObject.GetComponent<Outline>().enabled = false;
        }
    }

    public void DesactiveEveryOutline()
    {
        foreach (Transform item in transform)
        {
            item.gameObject.GetComponent<Outline>().enabled = false;
        }
    }
}
