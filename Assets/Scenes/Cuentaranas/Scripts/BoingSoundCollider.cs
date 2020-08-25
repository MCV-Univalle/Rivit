using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class BoingSoundCollider : MonoBehaviour
    {
        [SerializeField] private AudioClip boing1;
        [SerializeField] private AudioClip boing2;

        void OnTriggerEnter2D(Collider2D col)
        {
            if ((col.gameObject.tag == "Frog") && (col.gameObject.GetComponent<Frog>().IsJumping))
            {
                col.gameObject.GetComponent<JumpingFrogScript>().PlaySound(boing2);
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
