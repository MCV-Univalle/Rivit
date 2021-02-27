using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlidingPuzzle
{
    public static class ImageCropper
    {
        private static int _textureSize = 600;

        public static Sprite Crop(Texture2D image, int size, int x, int y)
        {
            int cropedSize = _textureSize / size;
            return Sprite.Create(image, new Rect(x * cropedSize, (size - 1 - y) * cropedSize, cropedSize, cropedSize), new Vector2(0.5f, 0.5f));
        }

    }
}