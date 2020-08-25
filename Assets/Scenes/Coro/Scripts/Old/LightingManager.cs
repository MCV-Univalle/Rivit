using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
namespace CoroMelodia
{
    public class LightingManager : MonoBehaviour
    {
        private static LightingManager _instance;
        public static LightingManager Instance
        {
            get
            {
                //Logic to create the instance
                if(_instance == null)
                {
                    GameObject go = new GameObject("LightingManager");
                    go.AddComponent<LightingManager>();
                    _instance = go.GetComponent<LightingManager>(); 
                }
                return _instance;
            }
        }
        [SerializeField]
        private GameObject _darkness;

        [SerializeField]
        private GameObject _frogIllumination;

        [SerializeField]
        private Color _redColor;
        [SerializeField]
        private Color _blueColor;
        [SerializeField]
        private Color _yellowColor;
        [SerializeField]
        private Color _purpleColor;
        [SerializeField]
        private Color _brownColor;
        [SerializeField]
        private Color _blackColor;

        [SerializeField]
        private Color _darkColor;
        [SerializeField]
        private Color _normalColor;

        private Color[] _lightColorList;
        private Renderer[] _frogsRendererList;
        private Vector3[] _frogsPositionList;

        void Awake()
        {
            _instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            _lightColorList = new Color[6];
            _lightColorList[0] = _redColor;
            _lightColorList[1] = _blueColor;
            _lightColorList[2] = _yellowColor;
            _lightColorList[3] = _purpleColor;
            _lightColorList[4] = _brownColor;
            _lightColorList[5] = _blackColor;
            //Get the render and position component for every frog
            //Assumptions: there are only 6 frogs
            _frogsRendererList = new Renderer[6];
            _frogsPositionList = new Vector3[6];
            GameObject[] frogList = FrogsManager.Instance.FrogList;
            for(int i = 0; i < frogList.Length; i++)
            {
                _frogsRendererList[i] = frogList[i].GetComponent<Renderer>();
                Vector3 frogPos = frogList[i].transform.position;
                frogPos.y += 2.4F; 
                _frogsPositionList[i] = frogPos;
            }
        }

        public void TurnLightsOff()
        {
            AudioManager.Instance.PlayLightOff();
            _darkness.gameObject.SetActive(true);
            foreach (var frogRender in _frogsRendererList)
            {
                frogRender.material.color = _darkColor;
            }
        }
        public void TurnLightsOn()
        {
            AudioManager.Instance.PlayLightOn();
            _darkness.gameObject.SetActive(false);
            _frogIllumination.GetComponent<Animator>().SetBool("on", false);
            foreach (var frogRender in  _frogsRendererList)
            {
                frogRender.material.color = _normalColor;
            }
        }

        public void IlluminateFrog(int num, bool val)
        {
            if(val)
            {
            _frogIllumination.GetComponent<Renderer>().material.color = _lightColorList[num];
            _frogIllumination.GetComponent<Animator>().SetBool("on", true);
            _frogIllumination.transform.position = _frogsPositionList[num];
            _frogsRendererList[num].material.color = _normalColor;
            }
            else 
            {
                _frogIllumination.GetComponent<Animator>().SetBool("on", false);
                _frogsRendererList[num].material.color = _darkColor;
            }   
        }
    }
}
*/