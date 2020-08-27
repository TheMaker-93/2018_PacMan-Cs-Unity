using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : MonoBehaviour {

    [Header("Referencias")]
    public GameObject gameManager;      // objeto de manager
    private ScoreManager scManager;     // objeto de gestion de puntuacion
    private GameManager gManager;       // manager del juego
    private LifesManager lfManager;     // manager de salud
    public MyGrid grid;                // grid del juego

    [Header("Position and Movement")]
    Node nextNode = new Node();     // nodo en el cual guardaremos el siguiente nodo antes de calcular si podemos o no ir a su posicion
    Node posibleNextNode = new Node();      // posible otro nodo de movimiento (hacia donde querremos ir al apretar una tecla)
    Node currentNode;           // nuestro nodo actual
    public Node initialNode;
    public Vector3 direction;          // direccion actual del pacman
    public enum MovementDirection
    {
        Up, Down, Left, Right, None
    }
    public MovementDirection currentDirection;
    public float speed;                // speed del pacMan
    private bool canMove;

    [Header("Sound")]
    public AudioClip movementSound;     // sonido producido al movernos
    private AudioSource audioSource;    // audiosource para el sondio de movimiento 
    public AudioClip deathSound;        // sonido de muerte
    public GameObject SFXLauncher;

    private bool movementSFXplaying;    // controlamos si el sonido de movimiento esta sonadno


	// Use this for initialization
	void Start () {

        // referencias
        scManager = gameManager.GetComponent<ScoreManager>();
        gManager = gameManager.GetComponent<GameManager>();
        lfManager = gameManager.GetComponent<LifesManager>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = movementSound;
        audioSource.loop = true;            // el sonido sera en bucle

        // maracamos al jugador como incapaz de moverse
        canMove = false;

        // guardmos el nodo en el que estamos
        currentNode = grid.GetNodeContainingPosition(transform.position);
        initialNode = currentNode;
        Debug.Log("The PacMan is on the node: " + currentNode.gridPosition_x + " :x  " + currentNode.gridPosition_y + " :y");

        // comporvamos si es transitable o no
        if (!currentNode.isTransitable)
        {
            Debug.LogError("ERROR DE POSICIONAMIENTO: PacMan no puede estar en este nodo intransitable",this.gameObject);
        }

        // snapeamos al nodo en el que estamos para que se coloque bien por si no esta bien puesto
        grid.AdjustPosition(this.gameObject, currentNode);

        // miramos si el sondio de movimiento esta sonando
        movementSFXplaying = GetComponent<AudioSource>().isPlaying;



	}
	
	// Update is called once per frame
	void Update () {

        if (canMove)
        {
            //Move();         // movimiento del pacman       
            ImprovedMovement();
            MovementSound();// sonido de movimiento
        } 

        // rotacion del personaje
        ChangeHeadRotation(currentDirection);

        // debug
        if (Input.GetKeyDown(KeyCode.Space))
        {
            lfManager.ReduceLifesLeft();
        }

        

    }

    public void Move()
    {
        // guardamos el input del jugador
        float x_input = Input.GetAxisRaw("Horizontal");
        float y_input = Input.GetAxisRaw("Vertical");

        // genermos una lista para los vecinos y al calcularlos los guardamos en esta
        List<Node> neighbours = new List<Node>();
        neighbours = grid.GetNeighnoursExtended(currentNode, false);
        Node nextNode = new Node();     // nodo en el cual guardaremos el siguiente nodo antes de calcular si podemos o no ir a su posicion

        // posicion en el mundo del nodo de destino
        Vector3 destinyPosition = new Vector3(0, 0, 0);           // AQUI GUARDARIAMOS LA POSICION DEL NODO AL QUE VAMOS

        // si estamos en la posicion del nodo y le damos a algun input
        if (transform.position.x == currentNode.position.x && transform.position.y == currentNode.position.y)
        {
            // debemos comprobar si en la direccion a la que quremos ir es o no transitable antes de cambiar de direccion

            if (x_input > 0f )
            {
                direction.x = 1;
                direction.y = 0;
                currentDirection = MovementDirection.Right;
            }
            else if (x_input < 0f)
            {
                direction.x = -1;
                direction.y = 0;
                currentDirection = MovementDirection.Left;
            }
            else if (y_input > 0f)
            {
                direction.x = 0;
                direction.y = 1;
                currentDirection = MovementDirection.Up;
            }
            else if (y_input < 0f)
            {
                direction.x = 0;
                direction.y = -1;
                currentDirection = MovementDirection.Down;
            }
        }


        // dependiendo de la direccion comprobamos los vecinos
        if (currentDirection == MovementDirection.Left || currentDirection == MovementDirection.Right)
        {
            nextNode = neighbours.Find(x => x.gridPosition_x == currentNode.gridPosition_x + direction.x);

        } else if (currentDirection == MovementDirection.Up || currentDirection == MovementDirection.Down)
        {
            nextNode = neighbours.Find(x => x.gridPosition_y == currentNode.gridPosition_y + direction.y);
        }

        // miramos el vecino y comporobamos si es o no transitable
        if (nextNode.isTransitable)
        {
            // guyardamos la posicion del siguiente nodo
            destinyPosition = nextNode.position;

            // cone sto nos movemos y nos paramos en la posicion que le pedimos
            transform.position = Vector3.MoveTowards(transform.position, destinyPosition, speed * Time.deltaTime);

            // si hemos llegado a la posicion del siguiente nodo lo guardamos como currentNode
            if (transform.position == destinyPosition)
            {
                currentNode = nextNode;
                //Debug.LogWarning("Hemos cambiado de ndo activo a : " + currentNode.gridPosition_x + " " + currentNode.gridPosition_y);
            }

        } else
        {

            Debug.LogWarning("el siguiente nodo NO es transitable");
            destinyPosition = currentNode.position;
            direction.x = 0;
            direction.y = 0;
            currentDirection = MovementDirection.None;

        }



        // debug
        Debug.DrawLine(transform.position, destinyPosition,Color.green);
    }


    private void ImprovedMovement()
    {
        // guardamos el input del jugador
        float x_input = Input.GetAxisRaw("Horizontal");
        float y_input = Input.GetAxisRaw("Vertical");

        // genermos una lista para los vecinos y al calcularlos los guardamos en esta
        List<Node> neighbours = new List<Node>();
        neighbours = grid.GetNeighnoursExtended(currentNode, false);

        // posicion en el mundo del nodo de destino
        Vector3 destinyPosition = new Vector3(0, 0, 0);           // AQUI GUARDARIAMOS LA POSICION DEL NODO AL QUE VAMOS

        // si estamos en la posicion del nodo y le damos a algun input
        if (transform.position.x == currentNode.position.x && transform.position.y == currentNode.position.y)
        {
            if (x_input > 0f)
            {
                // debemos comprobar si en la direccion a la que quremos ir es o no transitable antes de cambiar de direccion
                
                // comprobamos el vecino de la derecha
                posibleNextNode = neighbours.Find(x => x.gridPosition_x == currentNode.gridPosition_x + 1);
                // si resulta que el nodo hacia el cual queremos girar es transitable entonces giramos hacia ete
                if (posibleNextNode.isTransitable)
                {
                    direction.x = 1;
                    direction.y = 0;
                    currentDirection = MovementDirection.Right;
                }

            }
            else if (x_input < 0f)
            {

                posibleNextNode = neighbours.Find(x => x.gridPosition_x == currentNode.gridPosition_x - 1);

                if (posibleNextNode.isTransitable)
                {
                    direction.x = -1;
                    direction.y = 0;
                    currentDirection = MovementDirection.Left;
                }
            }
            else if (y_input > 0f)
            {
                posibleNextNode = neighbours.Find(x => x.gridPosition_y == currentNode.gridPosition_y + 1);

                if (posibleNextNode.isTransitable)
                {
                    direction.x = 0;
                    direction.y = 1;
                    currentDirection = MovementDirection.Up;
                }
                
            }
            else if (y_input < 0f)
            {

                posibleNextNode = neighbours.Find(x => x.gridPosition_y == currentNode.gridPosition_y - 1);

                if (posibleNextNode.isTransitable)
                {
                    direction.x = 0;
                    direction.y = -1;
                    currentDirection = MovementDirection.Down;
                }
                
            }
        }

        //posibleNextNode = null;

        // dependiendo de la direccion comprobamos los vecinos
        if (currentDirection == MovementDirection.Left || currentDirection == MovementDirection.Right)
        {
            nextNode = neighbours.Find(x => x.gridPosition_x == currentNode.gridPosition_x + direction.x);
        }
        else if (currentDirection == MovementDirection.Up || currentDirection == MovementDirection.Down)
        {
            nextNode = neighbours.Find(x => x.gridPosition_y == currentNode.gridPosition_y + direction.y);
        }


        // miramos el vecino y comporobamos si es o no transitable
        if (nextNode.isTransitable)
        {
            // guyardamos la posicion del siguiente nodo
            destinyPosition = nextNode.position;

            // cone sto nos movemos y nos paramos en la posicion que le pedimos
            transform.position = Vector3.MoveTowards(transform.position, destinyPosition, speed * Time.deltaTime);

            // si hemos llegado a la posicion del siguiente nodo lo guardamos como currentNode
            if (transform.position == destinyPosition)
            {
                currentNode = nextNode;
                //Debug.LogWarning("Hemos cambiado de ndo activo a : " + currentNode.gridPosition_x + " " + currentNode.gridPosition_y);
            }

        }
        else
        {

            Debug.LogWarning("el siguiente nodo NO es transitable");
            destinyPosition = currentNode.position;
            direction.x = 0;
            direction.y = 0;
            currentDirection = MovementDirection.None;

        }

        //Debug.DrawLine(transform.position, posibleNextNode.position, Color.blue);
        //Debug.DrawLine(transform.position, nextNode.position, Color.green);

    }

    // funcion privada puramente ornamental que cambiara la rotacion de la cabeza de pacMan dependiendo de su direccion
    private void ChangeHeadRotation(MovementDirection currentDirection)
    {
        switch (currentDirection)
        {
            case (MovementDirection.Up):
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            case (MovementDirection.Down):
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                break;
            case (MovementDirection.Right):
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                break;
            case (MovementDirection.Left):
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                break;
        }
    }
    // sonido de movimiento
    private void MovementSound ()
    {
        if (currentDirection != MovementDirection.None && !movementSFXplaying)
        {
            audioSource.Play(); // si nos estamos moviendo y el sonido no esta sonando hacemos que suene
            movementSFXplaying = true;  // marcamos que el sonido esta sonando

        }
        else if (currentDirection == MovementDirection.None)
        {
            audioSource.Stop();
            movementSFXplaying = false;
        }
    }

    // activacion o desactivacion del movimiento del pacman
    public void EnableOrDisableMovement (bool enabled)
    {
        canMove = enabled;
    }

    // colision con objetos mediante triggers
    private void OnTriggerEnter(Collider other)
    {
        //Debug.LogError("Colisionamos con " + other.transform.tag);

        // si se trata de una bola guardamos la puntuacion que nos da
        if (other.transform.tag == "Ball")
        {
            // incrementamos nuestra puntuacion
            // updateamos la informaicon en el gui de puntuacion
            //PlayerScore.IncreasePlayerScore(other.GetComponent<PunctuableItem>().GetScore());
            scManager.IncreaseScore(other.GetComponent<PunctuableItem>().GetScore());

            // reducimos el contador de bolas
            gManager.DecreaseBallCounter();

            // destruimos el elemento
            Destroy(other.gameObject);

            //Debug.Log(PlayerData.GetPlayerScore() + " SCORE") ;

        } else if ( other.transform.tag == "Fruit")
        {
            Debug.Log("Colisionamos con la fruta");

            // incrementamos nuestra puntuacion
            scManager.IncreaseScore(other.GetComponent<PunctuableItem>().GetScore());

            // destruimos el elemento
            Destroy(other.gameObject);

        } else if (other.transform.tag == "BigBall")
        {
            // en este caso podremos comer a los fantasmas durante un periodo de 10 segundos. Ademas estos entraran enmodo huida y cambiaran de color

            // accedemos al gamemanager
            gManager.StartGhostsRunAway();

            // reducimos el contador de bolas
            gManager.DecreaseBallCounter();

            // destruimos el objeto
            Destroy(other.gameObject);
        } 

        // comprobamos si hemos colisionado con algun fantasma y este es comible
        // si es comible cambiamos sue stado para que vuelva a la base y cambiamos el alfa de su material
        if (other.transform.tag == "Ghost")
        {
            // cuando podemos comernoslo
            if (other.GetComponent<GhostController>().eatable == true && other.GetComponent<GhostController>().eaten == false)
            {
                // cambiamos el alfa de su material
                // cambiamos su estado para que vuelva a la base
                // lo relalizamos llamando ala funcion del fantasma que iniciara este proceso
                other.GetComponent<GhostController>().BeEaten();

                Debug.LogError("Hemos colisionado con el fantasma comible" + other.transform.name);

            }

            if (other.GetComponent<GhostController>().eaten == false && other.GetComponent<GhostController>().eatable == false)
            {
                Debug.LogError("Nos han comido");

                //audioSource.clip = deathSound;
                //audioSource.Play();

                GameObject sfx =  Instantiate(SFXLauncher);
                sfx.GetComponent<AudioEffectController>().PlayAudioclip(deathSound);

                //StartCoroutine(PostDeathWait(deathSound.length));
                Invoke("lfManager.ReduceLifesLeft", deathSound.length);

                lfManager.ReduceLifesLeft();
                //this.gameObject.SetActive(false);


                // esto nos reduce la salud y quita una bola representativa de las visas que nos quedan
                // cambiamos el estado del juego a pregam
                //if (lfManager.lifesLeft >= 0)
                //{
                //    gManager.GoToPreGame();
                //    Debug.LogError("Vamos al pregame");
                //}
                // detemnemos el sonido
                audioSource.Stop();
                // miramos si el sondio de movimiento esta sonando pero ya sabemos que esta parado porque lo acabamos de parar
                movementSFXplaying = false;
            }



        } 


    }

    // para pedir el nodo en el que se encuentra
    public Node GetNode()
    {
        return currentNode;
    }
    
    // reseteamos la poscion del pacman a su posicion inicial
    public void ResetToStartingPosition()
    {
        canMove = false;
        currentNode = initialNode;
        transform.position = currentNode.position;
    }

    // funcion que esprara a que le sonido de muerte haya concludio para empezar la siguiente ronda
    IEnumerator PostDeathWait( float timeBeforeEnd)
    {
        Debug.LogWarning(timeBeforeEnd + "  " + Time.time);

        Debug.LogWarning("La corrutina funciona");
        yield return new WaitForSeconds(timeBeforeEnd);

        // cuando nos impactan y nos matan
        lfManager.ReduceLifesLeft();

        Debug.LogWarning("llegamos al final de la corrutina");

    }

}



// clase estatica que guardara la puntuacion del jugador entre escenas y durante el juego
// SOLO ACCESIBLE DESDE EL SCORE MANAGERR o el lifes manager
public static class PlayerData 
{
    public static float playerScore = 0;    // puntuacion acumulada (actual)
    public static int playerLifesLeft = 3;        // vidas restantes

    // usaremos esta funcion para acceder a la puntuacion del player desde el hud
    public static float GetPlayerScore ()
    {
        return playerScore;
    }
    // llamaremos a esta funcion para incrementar la puntuacnio del jugador
    public static void IncreasePlayerScore(float deltaScore)
    {
        playerScore += deltaScore;
    }

    // funcion para reducir las vidas restante del pacman
    public static void DecreaseLifes (int delta = 1)
    {
        playerLifesLeft -= delta;
    }

    public static bool LifesAvalible ()
    {
        Debug.LogError(playerLifesLeft);

        // si ya no nos quedan vidas y hemos agotado la actual
        if (playerLifesLeft <= 0)
        {
            return false;
        }
        // si nos quedan vidas notificamos de ello
        else
        {
            return true;
        }
    }


}