using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Home
{
    public class MainButtons : UIComponent
    {
        private void Start()
        {
            UIManager.gameListButtonEvent += FadeOut;
            UIManager.closeGameListButtonEvent += FadeIn;
        }
    }
}