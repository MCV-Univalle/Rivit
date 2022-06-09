using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CoroMelodia
{
    public class NumberNotesDisplayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI noteNumberText;
        public int Limit { get; set; }
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
            if (Limit > 0)
                noteNumberText.text = string.Format("{0}/{1}", num, Limit);
            else
                noteNumberText.text = num + "";
        }
    }
}