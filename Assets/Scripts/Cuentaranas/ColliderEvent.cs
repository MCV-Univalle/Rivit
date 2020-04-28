using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cuentaranas
{
    public class ColliderEvent : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D col)
            {
                if ((col.gameObject.tag == "Frog") && (col.gameObject.GetComponent<JumpingFrogScript>().IsJumping))
                {
                    col.gameObject.GetComponent<JumpingFrogScript>().PlaySound(true);
                }
            }
    }
}
