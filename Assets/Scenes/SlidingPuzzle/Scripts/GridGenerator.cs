using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlidingPuzzle
{
    public class GridGenerator : MonoBehaviour
    {
        [SerializeField] GameObject tilePrefab;

        public float GetRelativeGridScale(int size)
        {
            float width = Camera.main.orthographicSize * 2.0f * Screen.width / Screen.height;
            return width / (size + 0.8F);
        }

        public void AdjustGridScale(int size)
        {
            float newScale = GetRelativeGridScale(size);
            transform.localScale = Vector3.one * (newScale);
        }

        public void GenerateGrid(int size, Texture2D image)
        {
            GetComponent<GridLayoutGroup>().constraintCount = size;
            
            GetComponent<GridLayoutGroup>().enabled = true;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    InitializeTile(size, i, j, image);
                }
            }
        }

        private void InitializeTile(int size, int i, int j, Texture2D image)
        {
            int id = (i * size) + (j + 1);
            var tile = CreateTile();
            tile.Id = id;
            tile.gameObject.name = "[" + id + "]";
            tile.gameObject.GetComponent<SpriteRenderer>().sprite = ImageCropper.Crop(image, size, j, i);
        }

        private Tile CreateTile()
        {
            Tile tile = Instantiate(tilePrefab, transform).GetComponent<Tile>();
            tile.Locked = true;
            return tile;
        }
    }
}