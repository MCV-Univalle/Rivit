using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroMelodia
{
    public class Attention : MonoBehaviour
    {
        public void Activate(GameObject go)
        {
            gameObject.SetActive(true);
            GetComponent<CanvasGroup>().alpha = 0;
            Vector3 pos = go.GetComponent<Renderer>().transform.position;
            pos.x += 0.5F;
            pos.y += 0.55F;
            GetComponent<Renderer>().transform.position = pos;
            LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 1, 0.1F)
                .setEaseInOutCubic();
        }
        public void Desactivate()
        {
            LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 0, 0.1F);
            LeanTween.delayedCall(gameObject, 0.15F, () => gameObject.SetActive(false));
        }
    }
}