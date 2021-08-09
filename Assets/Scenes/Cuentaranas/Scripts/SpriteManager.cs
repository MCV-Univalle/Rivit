using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class SpriteManager
    {
        private SpriteRenderer spriteRenderer;
        bool _direction = false;

        public SpriteManager(SpriteRenderer renderer)
        {
            spriteRenderer = renderer;
        }

        public void Flip(Vector3 initialPoint, Vector3 finalPoint)
        {
            if (initialPoint.x < 0 || finalPoint.x > 0)
            {
                spriteRenderer.flipY = false;
            }
            else
            {
                spriteRenderer.flipY = true;
            }
        }

        public void Rotate(GameObject go, float rotZ, float time)
        {
            if (!_direction)
                rotZ *= -1;
            LeanTween.rotateZ(go, rotZ, time * 5);
        }
    }
}
