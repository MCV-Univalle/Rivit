using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;
using UnityEngine.UI;

namespace SlidingPuzzle
{
    public class ImageSelectionScreen : MonoBehaviour
    {
        [SerializeField] List<Texture2D> imagesList;
        [SerializeField] List<Texture2D> numberImagesList;
        [SerializeField] GameObject buttonPrefab;

        private SlidingPuzzleGameManager _gameManager;

        [Inject]
        public void Initialize(GameManager manager)
        {
            _gameManager = manager as SlidingPuzzleGameManager;
        }

        private void DestroyButtons()
        {
            foreach (Transform button in transform)
            {
                Destroy(button.gameObject);
            }
        }

        private void InstantiateButton(Texture2D item, int index)
        {
            var go = Instantiate(buttonPrefab, this.transform).GetComponent<ImageButton>();
            var sprite = Sprite.Create(item, new Rect(0, 0, item.width, item.height), new Vector2(0.5F, 0.5F));
            go.GetComponent<Image>().sprite = sprite;
            go.PuzzleImage = item;
            go.Index = index;
            go.ImageSelector = this;
        }

        public void GenerateButtons()
        {
            DestroyButtons();
            int index = 0;
            foreach (var item in imagesList)
            {
                InstantiateButton(item, index);
                index++;
            }
            int num = _gameManager.Size - 3;
            var item2 = numberImagesList[num];
            InstantiateButton(item2, index);

        }

        public void SelectImage(Texture2D image)
        {
            (_gameManager as SlidingPuzzleGameManager).SelectImage(image);
            LeanTween.alphaCanvas(transform.parent.GetComponent<CanvasGroup>(), 0, 0.2F);
            LeanTween.delayedCall(this.gameObject, 0.2F, () => transform.parent.gameObject.SetActive(false));
        }
    }
}