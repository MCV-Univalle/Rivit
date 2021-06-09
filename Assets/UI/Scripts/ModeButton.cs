using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeButton : MonoBehaviour
{
    public GameMode Mode{ get; set; }

    public void OnPressed()
    {
        GameObject.FindObjectOfType<UIManager>().SelectGameMode(Mode);
    }
}
