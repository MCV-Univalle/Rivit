using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlidingPuzzle
{
    public class ImageButton : MonoBehaviour
    {
        public int Index { get; set; }
        public ImageSelectionScreen ImageSelector { get; set; }
        public Texture2D PuzzleImage { get; set; } 

        public void SelectImage()
        {
            var image = this.GetComponent<Image>().sprite;
            ImageSelector.SelectImage(PuzzleImage);
        }
    }
}