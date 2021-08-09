using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CoroMelodia
{
    public class NumberNotesDisplayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI noteNumberText;
        private void Start()
        {
            Melody.updateNotes += UpdateNotes;
        }

        private void OnDestroy()
        {
            Melody.updateNotes -= UpdateNotes;
        }

        public void UpdateNotes(int num)
        {
            Debug.Log(noteNumberText);
            noteNumberText.text = num + "";
        }
    }
}