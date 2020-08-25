using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoroMelodia
{
    public class ChantScript : MonoBehaviour
    {
        public AudioSource audioSource;
        // Start is called before the first frame update
        void Awake()
        {
            audioSource = GetComponents<AudioSource>()[0];
        }

        public void PlayChant(AudioClip chant)
        {
            audioSource.clip = chant;
            audioSource.Play();
        }

        public void PauseChant()
        {
            audioSource.Stop();
        }

        public IEnumerator FadeOut(float fadeTime)
        {
            float initialVolume = audioSource.volume;
            while (audioSource.volume > 0)
            {
                audioSource.volume -= initialVolume * Time.deltaTime / fadeTime;
                yield return null;
            }
            audioSource.Stop();
            Destroy(gameObject);
        }
    }
}