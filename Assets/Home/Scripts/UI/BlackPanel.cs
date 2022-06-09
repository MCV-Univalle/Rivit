using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Home
{
    public class BlackPanel : UIComponent
    {
        void Start()
        {
            this.gameObject.SetActive(true);
            UIManager.gameListButtonEvent += FadeIn;
            UIManager.closeGameListButtonEvent += FadeOut;
            this.gameObject.SetActive(false);
        }
    }
}