using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowsManager : MonoBehaviour
{
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;

    public void VerifyLimits(int num, int length)
    {
        if((length == 0) || (length == 1))
        {
            rightArrow.SetActive(false);
            leftArrow.SetActive(false);
        }
        else if(num == 0)
        {
            leftArrow.SetActive(false);
            rightArrow.SetActive(true);
        }
        else if(num == length - 1)
        {
            rightArrow.SetActive(false);
            leftArrow.SetActive(true);
        }
        else
        {
            rightArrow.SetActive(true);
            leftArrow.SetActive(true);
        }
    }

    

}