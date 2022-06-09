using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CoroMelodia
{
    public class FrogsManager : MonoBehaviour
    {
        private List<GameObject> _frogsList;
        [SerializeField] private DirectorFrog _directorFrog;
        [Inject(Id = "SFXManager")] private AudioManager _SFXManager;
        public List<GameObject> FrogsList { get => _frogsList; set => _frogsList = value; }

        #region
        private void Start()
        {
            InitializeFrogsList();
            UIManager.executePlayButton += BlockFrogs;
            UIManager.executeHelpButton += BlockFrogs;
            UIManager.executePauseButton += BlockFrogs;
            UIManager.executeResumeFromPause += UnlockFrogs;
            UIManager.executeQuitGame += UnlockFrogs;
            UIManager.executeCloseModeSelectionButton += UnlockFrogs; 
            UIManager.executeCloseModeSelectionButton += ToDefault;
            UIManager.executeCloseInstructions += UnlockFrogs;
            UIManager.executeGameOver += BlockFrogs;
            UIManager.executeStartGame += UnlockFrogs;
            StartCoroutine(AppearEveryFrog(0.1F));
            
        }

        private void OnDestroy()
        {
            UIManager.executePlayButton -= BlockFrogs;
            UIManager.executeHelpButton -= BlockFrogs;
            UIManager.executePauseButton -= BlockFrogs;
            UIManager.executeResumeFromPause -= UnlockFrogs;
            UIManager.executeQuitGame -= UnlockFrogs;
            UIManager.executeCloseModeSelectionButton -= UnlockFrogs;
            UIManager.executeCloseModeSelectionButton -= ToDefault;
            UIManager.executeCloseInstructions -= UnlockFrogs;
            UIManager.executeGameOver -= BlockFrogs;
            UIManager.executeStartGame -= UnlockFrogs;
        }
        #endregion

        public void InitializeFrogsList()
        {
            _frogsList = new List<GameObject>();
            Transform frogs = gameObject.transform;
            foreach (Transform frog in frogs)
            {
                if (frog.gameObject.name != "Director")
                    _frogsList.Add(frog.gameObject);
            }
        }

        public void BlockFrogs()
        {
            foreach (var frog in _frogsList)
            {
                frog.GetComponent<Frog>().Enable = false;
            }
        }

        public void UnlockFrogs()
        {
            foreach (var frog in _frogsList)
            {
                frog.GetComponent<Frog>().Enable = true;
            }
        }

        public void Sing(int note, float speed)
        {
            _directorFrog.ChangePose();
            StartCoroutine(_frogsList[note].GetComponent<Frog>().Sing(speed));
        }

        public IEnumerator AppearEveryFrog(float waitTime)
        {
            yield return new WaitForSeconds(1F);
            foreach (var frog in _frogsList)
            {
                _SFXManager.PlayAudio("Boing");
                yield return new WaitForSeconds(waitTime);
            }
        }

        public void HideEveryFrog()
        {
            foreach (var frog in _frogsList)
            {
                frog.gameObject.SetActive(false);
            }
        }

        public void ToCelebrationPose()
        {
            DisplayMessage("¡Bien hecho!");
            _directorFrog.ToCelebrationPose();
        }

        public void DisplayMessage(string message)
        {
            _directorFrog.DisplayMessage(message);
        }

        public void DisplayMessage(string message, float time)
        {
            _directorFrog.DisplayMessage(message);
            LeanTween.delayedCall(gameObject, time, () => _directorFrog.HideMessage());
        }

        public void ShowCorrect(int num)
        {
            _directorFrog.ToFailPose();
            _frogsList[num].GetComponent<Frog>().TriggerSurprise();
        }

        public void ToDefault()
        {
            _directorFrog.ToDefault();
            foreach (var frog in _frogsList)
            {
                frog.GetComponent<Frog>().TriggerOffSurprise();
            }
        }

        public void ChangeColorFrog(int num, Color color)
        {
            _frogsList[num].GetComponent<Renderer>().material.color = color;
        }

        public void ChangeColorFrogs(Color color)
        {
            foreach (var frog in _frogsList)
            {
                frog.GetComponent<Renderer>().material.color = color;
            }
        }
    }
}