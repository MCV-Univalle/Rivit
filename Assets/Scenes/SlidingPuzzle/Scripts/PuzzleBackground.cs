using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SlidingPuzzle
{
    public class PuzzleBackground : MonoBehaviour
    {
        [SerializeField] GridGenerator gridGenerator;
        [SerializeField] ImageSelector imageSelector;
        SpriteRenderer spriteRenderer;
        

        private void Awake()
        {
            spriteRenderer = this.GetComponent<SpriteRenderer>();
            this.gameObject.SetActive(false);
        }

        public void ClearBackground()
        {
            this.gameObject.SetActive(true);
            spriteRenderer.color = new Color(1F, 1F, 1F, 1F);
        }

        public void DarkenBackground()
        {
            spriteRenderer.color = new Color(0.25F, 0.25F, 0.25F, 0.35F);
        }

        public void ChangeImage(Texture2D image)
        {
            spriteRenderer.sprite = ImageCropper.Crop(image, 1, 0, 0);
        }
    }
}