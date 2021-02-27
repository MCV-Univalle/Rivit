using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;

namespace SlidingPuzzle
{
    public class ImageSelector : MonoBehaviour
    {
        [SerializeField] List<Texture2D> imagesList;
        [SerializeField] ArrowsManager arrowsManager;
        [SerializeField] TextMeshProUGUI imageNumber;
        [SerializeField] PuzzleBackground puzzeBackground;
        [Inject] GameManager _gameManager;

        int _index = 0;

        public int Index { get => _index; set => _index = value; }

        public void Initialize()
        {
            SetCurrentIndex();
            VerifyIfStandardPuzzle();
        }

        public void ChangeImage(int direction)
        {
            _index += direction;
            puzzeBackground.ChangeImage(GetCurrentImage());
            SetCurrentIndex();
            VerifyIfStandardPuzzle();
        }

        public void SetCurrentIndex()
        {
            arrowsManager.VerifyLimits(_index, imagesList.Count);
            imageNumber.text = string.Format("{0}/{1}", _index + 1, imagesList.Count);
        }

        public void VerifyIfStandardPuzzle()
        {
            var manager = _gameManager as SlidingPuzzleGameManager;
            if (_index == 0)
            {
                manager.IsStandardPuzzle = true;
                manager.GenerateStandardPuzzle();
            }
                
            else
            {
                manager.IsStandardPuzzle = false;
                manager.DestroyTides();
            }
        }

        public Texture2D GetCurrentImage()
        {
            return imagesList[_index];
        }
    }
}