using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class InstructionsScreen : UIComponent
{
    [SerializeField] private TextMeshProUGUI pageNumber;
    [SerializeField] private GameObject pagesContainer;
    [SerializeField] private ArrowsManager arrowsManager;
    private int _index;
    [SerializeField]  private UIManager _uiManager; 

    void Start()
    {
        positionY = -675;
        positionX = 0;
        UIManager.executeHelpButton += ShowCurrentPage;
        UIManager.executeCloseInstructions += FadeOutMoveY;

        DesactivePages();
        UpdatePageNumber(_index, pagesContainer.transform.childCount);
        arrowsManager.VerifyLimits(_index, pagesContainer.transform.childCount);
    }

    public void MovePage(int direction)
    {
        Transform pages = pagesContainer.transform;
        pages.GetChild(_index).GetComponent<InstructionPage>().MovePageOut(direction);
        _index += direction;
        pages.GetChild(_index).gameObject.SetActive(true);
        pages.GetChild(_index).GetComponent<InstructionPage>().MovePageIn(direction);
        UpdatePageNumber(_index, pages.childCount);
        arrowsManager.VerifyLimits(_index, pages.childCount);
    }

    public void DesactivePages()
    {
        Transform pages = pagesContainer.transform;
        foreach (Transform page in pages)
        {
            page.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            page.gameObject.SetActive(false);
        }
    }

    public void ShowCurrentPage()
    {
        Transform pages = pagesContainer.transform;
        pages.GetChild(_index).gameObject.GetComponent<CanvasGroup>().alpha = 1;
        pages.GetChild(_index).gameObject.SetActive(true);
        pages.GetChild(_index).transform.localPosition = new Vector3(0, 0, 0);
        FadeInMoveY();
    }


    public void UpdatePageNumber(int currentPage, int totalPages)
    {
        pageNumber.text = currentPage + 1 + "/" + totalPages;
    }

    void OnDestroy()
    {
        UIManager.executeHelpButton -= ShowCurrentPage;
        UIManager.executeCloseInstructions -= FadeOutMoveY;
    }
}
