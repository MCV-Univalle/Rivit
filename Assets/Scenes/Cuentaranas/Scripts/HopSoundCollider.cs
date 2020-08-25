using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cuentaranas
{
    public class HopSoundCollider : MonoBehaviour
    {
        [SerializeField] private AudioClip boing1;
        [SerializeField] private AudioClip boing2;

        private void Start()
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, 15); 
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if ((col.gameObject.tag == "Frog") && (col.gameObject.GetComponent<Frog>().IsJumping))
            {
                col.gameObject.GetComponent<Frog>().PlaySound(boing2);
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if ((col.gameObject.tag == "Frog") && (col.gameObject.GetComponent<Frog>().IsJumping))
            {
                col.gameObject.GetComponent<Frog>().PlaySound(boing1);
            }
        }
    }
}
