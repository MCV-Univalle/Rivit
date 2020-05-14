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
            UIManager.Instance.SetVisibilidadTextTimer(false);


            // Nueva version V2 de los saltos de la rana amarilla

            Shuffle(arrayRutaRanaAmarilla); // Reordena el orden de los elementos de un array

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

            
            //tAG.setPosPLayer(leafController.getPositionLeave(rutaIniRana[0]));

            base.Start();
        }
        //------------------------------------------------------------Update -------------------------------------------------------
        // Update is called once per frame
        void Update()
        {
            if(iniciarJuego)
            {
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

                validacionDeListas2();

                if (finJuego == 1)
                {
                    ShowResults();
                }

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

        Vector3 posLeaveReturn = new Vector3(-0.78f, 3f, 0);

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
        private int finJuego = 0;

        [SerializeField]
        private float tiempoEsperaSaltoCom;
        private float tiempo = 0.0f;
        //--------------------------------------------------------------------

        public override void StartGame()
        {
            GameManager.Instance.Score = 0;
            finJuego = 0;
            AnimationManager.Instance.SetDireccionSaltoPlayer(100);

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

            GenerarListaAleatoriaRutaLibelula();

            Debug.Log("StartGame");
        }

        public override void FinishGame()
        {
            Debug.Log("touchHistory.count = "+ touchHistory.Count);
            if (touchHistory.Count == 8)
            {
                AudioManagerSaltos.Instance.PlayWinGame();
            }
            else
            {
                AudioManagerSaltos.Instance.PlayGameOver();
            }

            iniciarJuego = false;
            playerTAG.setPosPLayer(new Vector3(-0.78f, 3f, 0));
            AnimationManager.Instance.SetDireccionSaltoPlayer(100);  // posicion de la animacion inicial
            comTAG.setPosPLayer(new Vector3(-5.5f, 1f, 0));

            Debug.Log("FinishGame");
        }

        public override void ShowResults()
        {
            FinishGame();
            base.ShowResults();
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
            tiempo += Time.deltaTime;

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
                //Verifica si ha trancurrido el tiempo de retraso
                //para despues 
                if (tiempo > tiempoEsperaSaltoCom)
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

                    // Remove the recorded 2 seconds.
                    tiempo = tiempo - tiempoEsperaSaltoCom;

                }
            }

            //Debug.Log("var isMoving: " + tAG.isMoving + ", count: " + contRutaRana);
        }

        private void SigSaltoCom(bool activar)
        {
            if (activar && contRutaRana <= arrayRutaRanaAmarilla.Length && contRutaRana >= 1)
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

        /*
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
        */

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
                    // Refresca el valor del Score
                    GameManager.Instance.Score += (touchHistory.Count +1) * 3;

                    touchHistory.Add(leafController.getIndexLeave(wp)); // agrega a la lista el indice de la planta tocada

                    //Realiza el siguiente salto de la rana amarilla
                    sigSaltoComAct = true;
                    if (contRutaRana <= (arrayRutaRanaAmarilla.Length - 1)) contRutaRana += 1;
                    //Debug.Log("contRutaRana =" + contRutaRana);


                    if (TouchAndGo.Instance.player.name == "Player")
                    {
                        int angulo = (int) TouchAndGo.Instance.AngleInDeg(TouchAndGo.Instance.player.transform.position, posLeaveReturn);
                        int numSalto = 100;

                        if (angulo > 120 && angulo < 160) numSalto = 0;    // arriba izquierda
                        if (angulo > 60 && angulo < 120) numSalto = 1;    // arriba
                        if (angulo > 20 && angulo < 60) numSalto = 2;    // arriba derecha

                        if (angulo > 160 && angulo > -160) numSalto = 3;  // izquierda
                        if (angulo < 20 && angulo > -20) numSalto = 5;    // derecha

                        if (angulo < -120 && angulo > -160) numSalto = 6;  // abajo izquierda
                        if (angulo > -120 && angulo < -60) numSalto = 7;  // abajo
                        if (angulo < -20 && angulo > -60) numSalto = 8;  // abajo derecha

                        AnimationManager.Instance.SetDireccionSaltoPlayer(numSalto);
                        //Debug.Log("Angulo de salto de " + TouchAndGo.Instance.player.name + " = " + angulo + ", Animacion Numero = " + numSalto);
                    }

                    //validacionDeListas2();
                }

            }

        }

 
        public void validacionDeListas2()
        {
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
                    finJuego += 1;
                    Debug.Log("[ValidacionDeListas2] Fin del juego: " + ecual);
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

        private void GenerarListaAleatoriaRutaLibelula()
        {
            Shuffle(arrayRutaRanaAmarilla); // Reordena el orden de los elementos de un array

            /*
            // Genera numeros aleatorios y los guarda en una lista
            numeroAleatorio = 0;
            rutaIniRana.Clear();

            while (rutaIniRana.Count <= 3)
            {
                numeroAleatorio = Random.Range(1, 8);

                //Sólo si el número generado no existe en lalista se agrega
                if (!rutaIniRana.Contains(numeroAleatorio))
                {
                    rutaIniRana.Add(numeroAleatorio);
                }
            }
            */
        }


    }
}
