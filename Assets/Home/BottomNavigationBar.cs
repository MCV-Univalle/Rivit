using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BottomNavigationBar : MonoBehaviour
{
    private List<GameObject> buttons;
    private int activeIndex = 0;
    [SerializeField] private Color inactiveColor;
    [SerializeField] private GameObject panels;

    void Start()
    {
        buttons = new List<GameObject>(transform.childCount);
        foreach (Transform child in transform)
        {
            buttons.Add(child.gameObject);
            InactivateButton(child);
        }
        ActivateButton(buttons[activeIndex].transform);
    }

    private void ChangeButtonColor(Transform button, Color color)
    {
        button.GetChild(0).GetComponent<Image>().color = color;
        //button.GetChild(1).GetComponent <TextMeshProUGUI>().color = color;
    }

    private void ActivateButton(Transform button)
    {
        ChangeButtonColor(button, Color.black);
        button.GetComponent<Button>().interactable = false;
    }

    private void InactivateButton(Transform button)
    {
        ChangeButtonColor(button, inactiveColor);
        button.GetComponent<Button>().interactable = true;
    }

    public void OnPressed(int index)
    {
        InactivateButton(buttons[activeIndex].transform);
        panels.transform.GetChild(activeIndex).gameObject.SetActive(false);
        activeIndex = index;
        ActivateButton(buttons[activeIndex].transform);
        panels.transform.GetChild(activeIndex).gameObject.SetActive(true);
    }
}
