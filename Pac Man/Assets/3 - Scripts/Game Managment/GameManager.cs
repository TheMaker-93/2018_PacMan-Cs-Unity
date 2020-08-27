using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("References")]
    private SoundManager soundManager;
    private SceneManagment scnManager;
    public PacMan player;
    public GameObject ghostsContainer;      // contenedor de los fantasmas
    public GhostController[] ghController; // controladores de los fantasmas

    [Header ("Balls")]
    public GameObject smallBallsContainer;      // contenedor con las bolas pequeñas
    public int remainingBalls;                  // bolas restantes

    [Header("Game Status")]
    public GameStatus currentGameStatus;
    public enum GameStatus
    {
        preLevel,       // suena la musica inicial del juego y cuando acaba pasamos al onLevel
        onLevel,        // fase principal del juego, nos movemos e interactuamos como se espera
        endLevel        // fase final del nivel, se activa cuando hemos comido todas las bolas
    }

    // timers
    public float endGame_VictoryTime = 3f;                   // tiempo que durara la fase de endGame VICOTIRIOSA
    public float endGame_DefeatTimee = 3f;                   // tiempo que durara la fase de endGame DERROTA
    private float remainingTimeForSceneChange;         // tiempo que queda hasta que pasemos de escena

    public bool victorious;

    // Use this for initialization
    void Awake () {

        // referencias
        soundManager = GetComponent<SoundManager>();
        scnManager = GetComponent<SceneManagment>();

        // guardamos los controladores de los fantasmas
        ghController = new GhostController[ghostsContainer.transform.childCount];
        for (int i = 0; i < ghostsContainer.transform.childCount; i++)
        {
            ghController[i] = ghostsContainer.transform.GetChild(i).GetComponent<GhostController>();
        }

        // guardamos cuantas bolas hay en total
        remainingBalls = smallBallsContainer.transform.childCount;
        // setamos el estado de juego a preLevel
        currentGameStatus = GameStatus.preLevel;
        

	}
	
	// Update is called once per frame
	void Update () {

        // maquina de estados del juego
        GameStateMachine();

        // debug
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeStatusToEndgame(true);
        }

    }

    public void GameStateMachine ()
    {
        switch (currentGameStatus)
        {
            case (GameStatus.preLevel):

                // el jugador no podra moverse
                player.EnableOrDisableMovement(false);
                // los fantasmas no podran moverse
                for (int i = 0; i < ghController.Length; i++)
                {
                    ghController[i].EnableOrDisableMovement(false);
                }

                // si no hay sonido sonando inicialmente lo ejecutamos
                if (soundManager.LaunchInitialGameSound() == false)
                {
                    // si el sonido inicial ya ha acabado
                    currentGameStatus = GameStatus.onLevel;
                }

                break;
            case (GameStatus.onLevel):

                // activamos el movimiento del personaje
                player.EnableOrDisableMovement(true);

                // activamos el movimiento de los fantasmas
                for (int i = 0; i < ghController.Length; i++)
                {
                    ghController[i].EnableOrDisableMovement(true);
                }

                // comprobamos si quedan bolas por comer, si no quedan, pasamos al siguiente estado
                // CheckEndLevel();
                // ESTO LO HACEMOS CADA VEZ QUE COGEMOS UNA BOLA

                break;
            case (GameStatus.endLevel):


                // dependera de si hemos ganado o no lo que succedera


                if (victorious == true)
                {

                    Debug.LogError("Has vencido, cargando siguiente nivel");

                    remainingTimeForSceneChange += Time.deltaTime;
                    if (remainingTimeForSceneChange >= endGame_VictoryTime)
                    {
                        // cargamos el siguiente nivel
                        scnManager.LoadNextLevel();
                    }

                } else 
                {
                    Debug.LogError("Has perdido, reiniciando nivel");
                    remainingTimeForSceneChange += Time.deltaTime;
                    if (remainingTimeForSceneChange >= endGame_DefeatTimee)
                    {
                        // volvemos a cargar este nivel
                        // NO
                        //scnManager.ReloadCurrentLevel();
                        Debug.LogWarning("Saliendo del juego");
                        Debug.Break();

                    }
                }


                break;
        }
    }

    // cambioms externos de etsado de jugo
    public void GoToPreGame()
    {

        player.ResetToStartingPosition();            // resetamos la posicion del player

        foreach (GhostController ghost in ghController)
        {
            ghost.ResetInitialPosition();               // resetamos la posicion de los fantasmas
        }



        currentGameStatus = GameStatus.preLevel;
    }



    // reducimos el valor que nos indica cuantas bolas hay
    public void DecreaseBallCounter (int delta = 1)
    {
        remainingBalls -=  Mathf.Abs(delta);

        CheckEndLevel();
    }
    // chequeamos que no queden bolas
    private void CheckEndLevel ()
    {
        if (remainingBalls <= 0)
        {
            Debug.LogError("Nivel Completado");
            // cambiamos al estado final siendo victoriosos
            ChangeStatusToEndgame(true);

        }
    }

    // cambiamos el estado del juego 
    public void ChangeStatusToEndgame( bool _victorious)
    {
        // 

        // setamos si somos o no victoriosos
        victorious = _victorious;
        // pasamos al modo final 
        currentGameStatus = GameStatus.endLevel;

        // mostramos los elementos de interfaz que muestren si hemos ganado o perdido


    }

    // hacemos que todos los fantasmas cambien al estado de huida
    public void StartGhostsRunAway()
    {
        
        for (int i = 0; i < ghController.Length; i++)
        {

                ghController[i].InitiateRunAwayState();
            
        }
        
    }
    
}
