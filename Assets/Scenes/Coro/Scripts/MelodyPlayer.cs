using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CoroMelodia
{
    public class MelodyPlayer : MonoBehaviour
    {
        [SerializeField] Melody _melody;
        [SerializeField] private FrogsManager _frogsManager;
        [SerializeField]private DirectorFrog _directorFrog;
        [SerializeField] private Light _light;
        
        [Inject(Id = "SFXManager")] private AudioManager _SFXManager;
        public float Speed { get; set; }
        public bool IsReadyToStartMelody { get; set; }

        public IEnumerator PrepareToStartMelody(string message)
        {
            _frogsManager.BlockFrogs();
            yield return new WaitForSeconds(0.3F);
            _directorFrog.DisplayMessage(message);
            for (int i = 0; i < 3; i++)
            {
                _SFXManager.PlayAudio("MetronomeClap");
                yield return new WaitForSeconds(Speed);
            }
            _SFXManager.PlayAudio("LightOff");
            _directorFrog.Prepare();
            _directorFrog.HideMessage();
            _light.SwitchEnvironmentLight(false);
            yield return new WaitForSeconds(Speed);
            IsReadyToStartMelody = true;
        }

        public IEnumerator PlayMelody()
        {
            List<int> notes = _melody.Notes;
            foreach (int note in notes)
            {
                _light.IlluminateIndividualFrog(note);
                _frogsManager.Sing(note, Speed);
                yield return new WaitForSeconds(Speed);
                _light.TurnOffIndividualLight(note);
                yield return new WaitForSeconds(0.1F);

            }
            yield return new WaitForSeconds(0.2F);
            FinishMelody();
        }
        public void FinishMelody()
        {
            _frogsManager.UnlockFrogs();
            _light.SwitchEnvironmentLight(true);
            _directorFrog.ToDefault();
            _SFXManager.PlayAudio("LightOn");
            _frogsManager.DisplayMessage("Ahora repítela tú", 1F);
        }
    }
}