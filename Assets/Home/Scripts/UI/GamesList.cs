using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Home
{
    public class GamesList : UIComponent
    {
        void Start()
        {
            positionY = -475;
            UIManager.gameListButtonEvent += FadeInMoveY;
            UIManager.closeGameListButtonEvent += FadeOutMoveY;
        }
    }
}