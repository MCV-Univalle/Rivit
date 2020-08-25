using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class MainScreen : MonoBehaviour
    {
        [SerializeField] FrogsManager frogsManager;

        private void Start()
        {
            UIManager.executePlayButton += Stop;
            UIManager.executeHelpButton += Stop;
            UIManager.executeCloseModeSelectionButton += MakeJumpFrogs; 
            UIManager.executeCloseInstructions += MakeJumpFrogs;

            MakeJumpFrogs();

        }
        private void OnDestroy()
        {
            UIManager.executePlayButton -= Stop;
            UIManager.executeHelpButton -= Stop;
            UIManager.executeCloseModeSelectionButton -= MakeJumpFrogs; 
            UIManager.executeCloseInstructions -= MakeJumpFrogs;
            StopAllCoroutines();
        }
            
        private void MakeJumpFrogs()
        {
            frogsManager.ActiveFrogsNumber = 3;
            StartCoroutine(frogsManager.StartJumping(100000, 1.5F, 0.6F, 0.055F));
        }
        private void Stop()
        {
            StopAllCoroutines();
        }
    }
}
