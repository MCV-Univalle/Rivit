using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saltos
{
    public class GameManager : GameController
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                //Logic to create the instance
                if (_instance == null)
                {
                    _instance = new GameManager();
                }
                return _instance;
            }
        }

        void Awake()
        {
            _instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            //Time.timeScale = 0;

            // Nueva version V2 de los saltos de la rana amarilla

            Shuffle(arrayRutaRanaAmarilla); // Reordena el orden de los elementos de un array
                                            //--------------------------------


            // Genera numeros aleatorios y los guarda en una lista
            numeroAleatorio = 0;

            while (rutaIniRana.Count <= 3)
            {
                numeroAleatorio = Random.Range(1, 8);

                //Sólo si el número generado no existe en lalista se agrega
                if (!rutaIniRana.Contains(numeroAleatorio))
                {
                    rutaIniRana.Add(numeroAleatorio);
                }
            }

            contRutaRana = 9; // Hace que no inicie el recorido la rana amarilla
                              //tAG.isMoving = true;

            // posiciona la rana amarilla en la posicion de la hoja que esta primera en la secuencia del recorrido
            //tAG.setPosPLayer(leafController.getPositionLeave(rutaIniRana[0]));
            base.Start();


            
        }
        //------------------------------------------------------------Update -------------------------------------------------------
        // Update is called once per frame
        void Update()
        {
            if(iniciarJuego)
            {
                //Debug.Log("Entrooooo eh eh eh epa colombia!!!!");

                // Verifiaca si se ha tocado el boton "inicio"
                if (UIc.isClikButtonInicio() || empezarJuego)
                {
                    //Time.timeScale = 1;

                    contRutaRana = 0;
                    comTAG.isMoving = true;

                    touchHistory.Clear(); // borra todos los elementos de la lista
                    UIc.activeCheckTrueOrFalse(false, false);

                    UIc.setButtonInicio(false);
                    UIc.buttonVisible(false);
                }

                RecorridoInicialRanaAmarilla();

                if (arrayRutaRanaAmarilla.Length >= 4)
                {
                    //entradaTouch();
                    ImputClick();



                    // Mueve la rana hacia un punto especifico
                    playerTAG.moveToPoint(posLeaveReturn);
                    SigSaltoCom(sigSaltoComAct);
                }
                //validacionDeListas2();

                UIManager.Instance.SetCanSaltosText(touchHistory.Count);
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------
        //-------------- VARIABLES ----------------------------------------------
        public LeafController leafController = new LeafController();
        public TouchAndGo comTAG = new TouchAndGo(); // libelula
        public TouchAndGo playerTAG = new TouchAndGo(); // rana naranja
        public Saltos.UIManager UIc = new Saltos.UIManager();

        public Touch touch;

        Vector3 posLeaveReturn;

        public List<int> rutaIniRana = new List<int>();

        [SerializeField]
        int contRutaRana = 0;

        [SerializeField]
        public List<int> touchHistory = new List<int>();

        [SerializeField]
        int[] arrayRutaRanaAmarilla = { 1, 2, 3, 4, 5, 6, 7, 8 }; // representa las plantas
        private System.Random _random = new System.Random();

        int numeroAleatorio;

        bool sigSaltoComAct = false;

        public bool empezarJuego = false;

        int counterClick;

        private bool timerStart = false;
        private bool iniciarJuego = false;
        //--------------------------------------------------------------------

        public override void StartGame()
        {
            contRutaRana = 0;
            comTAG.isMoving = true;

            touchHistory.Clear(); // borra todos los elementos de la lista
            UIc.activeCheckTrueOrFalse(false, false);

            UIc.setButtonInicio(false);
            UIc.buttonVisible(false);

            counterClick = 0;

            UIManager.Instance.targetTime = 3f;

            timerStart = true;

            iniciarJuego = false;

            StartCoroutine(ExampleCoroutine());

            Debug.Log("Empezó el juego");
        }

        public override void FinishGame()
        {
            Debug.Log("FinishGame");
        }

        public override void ShowResults()
        {
            Debug.Log("ShowResults");
        }

        public override void AdaptGameParameters() //Adjus the game parametes according to the game mode
        {
            Debug.Log("AdaptGameParameters");
        }

        //-------------- INICIO -> METODOS DE LA MECANICA DEL JUEGO ---------------
        public void RecorridoInicialRanaAmarilla()
        {
            int cantSaltos = 4;

            // Realiza el recorrido de la rana de color amarillo
            if (comTAG.isMoving && contRutaRana <= cantSaltos - 1)
            {
                //int num = rutaIniRana[contRutaRana];
                int num = arrayRutaRanaAmarilla[contRutaRana];

                //Debug.Log("Pocion en el array de ruta: " + num);

                comTAG.moveToPoint(leafController.getPositionLeave(num));
            }
            else
            {
                if (contRutaRana <= cantSaltos - 1)
                {
                    comTAG.isMoving = true;
                    contRutaRana += 1;
                }
                else
                {
                    comTAG.isMoving = false;
                }
            }

            //Debug.Log("var isMoving: " + tAG.isMoving + ", count: " + contRutaRana);

        }

        private void SigSaltoCom(bool activar)
        {
            if (activar && contRutaRana <= arrayRutaRanaAmarilla.Length)
            {
                int num = arrayRutaRanaAmarilla[contRutaRana - 1];
                comTAG.moveToPoint(leafController.getPositionLeave(num));

                //Debug.Log("sigSaltoComAct=" + sigSaltoComAct);
            }
            else
            {
                sigSaltoComAct = false;
                //Debug.Log("sigSaltoComAct=" + sigSaltoComAct);
            }
        }


        public void entradaTouch()
        {
            // Detecta si ha realizado un toque en la pantalla tactil
            if (Input.touchCount > 0 && playerTAG.isMoving == false && contRutaRana >= 4)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 pos = touch.position;
                //Debug.Log(pos);

                Vector3 wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                posLeaveReturn = leafController.isTouchLeaves(wp);

                //int indicePlanta = leafController.getIndexLeave(wp);

                //Debug.Log("Pos: " + posLeaveReturn +", index: " + indicePlanta);


                // Realiza la accion cuando se levanta el dedo de la pantalla
                if (leafController.seTocoLeave(wp))
                {
                    touchHistory.Add(leafController.getIndexLeave(wp)); // agrega a la lista el indice de la planta tocada

                    //Realiza el siguiente salto de la rana amarilla
                    sigSaltoComAct = true;
                    if (contRutaRana <= (arrayRutaRanaAmarilla.Length - 1)) contRutaRana += 1;
                    Debug.Log("contRutaRana =" + contRutaRana);

                    validacionDeListas2();
                }


            }
        }


        void ImputClick()
        {
            /*if ( )
            {
                counterClick += 1;

                //Debug.Log("Contador de clicks = " + counterClick);
                //Debug.Log("Pressed primary button.");
                //Debug.Log(Input.mousePosition);
            }*/

            //Debug.Log("Condicion del if = " + (Input.touchCount > 0  && playerTAG.isMoving == false && contRutaRana >= 4));
            // Detecta si ha realizado un toque en la pantalla tactil
            if (Input.GetMouseButtonDown(0) && playerTAG.isMoving == false && contRutaRana >= 4)
            {
                

                Vector2 pos = Input.mousePosition;
                //Debug.Log(pos);

                Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                posLeaveReturn = leafController.isTouchLeaves(wp);

                //int indicePlanta = leafController.getIndexLeave(wp);

                //Debug.Log("Pos: " + posLeaveReturn );
                

                // Realiza la accion cuando se levanta el dedo de la pantalla
                //if (touch.phase == TouchPhase.Began && leafController.seTocoLeave(wp))

                if (leafController.seTocoLeave(wp))
                {
                    touchHistory.Add(leafController.getIndexLeave(wp)); // agrega a la lista el indice de la planta tocada

                    //Realiza el siguiente salto de la rana amarilla
                    sigSaltoComAct = true;
                    if (contRutaRana <= (arrayRutaRanaAmarilla.Length - 1)) contRutaRana += 1;
                    //Debug.Log("contRutaRana =" + contRutaRana);

                    //validacionDeListas2();
                }

            }

        }

        public void validacionDeListas()
        {
            bool activacion = false;
            bool ecual = false;
            int count = 0;

            if (touchHistory.Count == arrayRutaRanaAmarilla.Length && contRutaRana < arrayRutaRanaAmarilla.Length)
            {
                foreach (int element in arrayRutaRanaAmarilla)
                {
                    if (element == touchHistory[count])
                    {
                        ecual = true;
                    }
                    else
                    {
                        ecual = false;
                        break;
                    }
                    count += 1;
                }

                Debug.Log("Fin del juego: " + ecual);
                UIc.activeCheckTrueOrFalse(true, ecual);
                UIc.buttonVisible(true);

                UIc.buttonRecargarVisible(true);

                Time.timeScale = 0;
            }

        }

        public void validacionDeListas2()
        {
            bool activacion = false;
            bool ecual = false;
            int count = 0;

            if (touchHistory.Count <= arrayRutaRanaAmarilla.Length && contRutaRana <= arrayRutaRanaAmarilla.Length && touchHistory.Count > 0)
            {
                foreach (int element in touchHistory)
                {
                    if (element == arrayRutaRanaAmarilla[count])
                    {
                        ecual = true;
                    }
                    else
                    {
                        ecual = false;
                        break;
                    }
                    count += 1;
                }

                if (ecual == false || (ecual == true && touchHistory.Count == arrayRutaRanaAmarilla.Length))
                {
                    Debug.Log("Fin del juego: " + ecual);
                    UIc.activeCheckTrueOrFalse(true, ecual);
                    UIc.buttonVisible(true);

                    UIc.buttonRecargarVisible(true);

                    Time.timeScale = 0;
                }

            }

        }

        // Recibe una lista y retorna una lista con los elementos desordenados
        void Shuffle(int[] array)
        {
            int p = array.Length;
            for (int n = p - 1; n > 0; n--)
            {
                int r = _random.Next(1, n);
                int t = array[r];
                array[r] = array[n];
                array[n] = t;
            }
        }

        public void Recargar()
        {
            Application.LoadLevel("Saltos");
            // Nueva version V2 de los saltos de la rana amarilla

            Shuffle(arrayRutaRanaAmarilla); // Reordena el orden de los elementos de un array
                                            //--------------------------------
        }
        //-------------- FIN -> METODOS DE LA MECANICA DEL JUEGO ---------------

        IEnumerator ExampleCoroutine()
        {
            while (timerStart && UIManager.Instance.targetTime >= 0)
            {
                yield return new WaitForSeconds(1);
                UIManager.Instance.SetValTimerText(UIManager.Instance.targetTime);
                UIManager.Instance.targetTime -= 1;

                //Debug.Log("targetTime : " + UIManager.Instance.targetTime);
            }

            iniciarJuego = true;

        }


    }
}
