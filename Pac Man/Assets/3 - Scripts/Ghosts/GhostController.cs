using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// gameactor tendra las funcones de movimiento, 
public class GhostController : MonoBehaviour {

    [Header("Referencias")]
    private MyGrid grid;         // grid del juego
    public Node currentNode = new Node();    // nodo actual
    private PacMan pacMan;       // referencia al pac man
    public Node initialNode = new Node();     // nodo de inicio

    [Header("Movement")]
    private bool canMove = false;               // controlamos si podems o no movernos
    public bool eatable = false;               // nos lo podemos comer?
    public bool eaten = false;                  // nos han comido?
    // ANTES NECESITAS TENER LA RECURSIVA HECHA
    public List<Node> pathToFollow;        // camino que seguira el fantasma hasta el waypoint objetivo
    public float speed;                     // velocidad de movimiento 
    public int distanceToPacman;
    private GameObject ghostBaseObject;            // base de los fantasmas 
    private Node ghostBase;
    private bool onBase;                            // marcamos si estamos en la base 
    // color que tendra el camino
    public Color pathColor;
    //

    [Header("IA preferences")]
    public Waypoint[] waypoints;               // lista de waypoints que recorreremos (orden de colocacion = orden de ejecucion)
    public int currentWaypointIndex;            // indice del waypoint al cual nos dirigimos
    public Node nodeToReach = new Node();                    // nodo al que trataremos de llegar
    public Node nextNode = new Node();                       // nodo vecino al cual nos movemos
    public enum MovementType
    {
        waypointMode, chase, runAway,eaten,returningToGame
    }
    public MovementType movementType = MovementType.waypointMode;       // TIPO DE MOVIMIENTO POR DEFECTO
    private bool pathCalculated;                            // variable que nos indicara si ya hemos calculado el camino para el nodo actual
    public int maxChaseDistance;                          // distancia maxima a la cual el fantasma optara por perseguri al pacman, una vez superada vovlera l modo waypoint

    // variable spara el comportamiento de cada IA
    float destinyOffset = 0f;          // variable que se sumara a la posicion del pacman y su direccion para hacer que un fantasma se adelante a donde va el pacman

    // materiales
    //private GameObject ghostBaseMesh;               // objeto fisico que forma el fantasma
    public Renderer ghostRenderer;     // rendedrer del objeto 
    public Color initialColor;              // Color con el que empezaremos 
    public Color runAwayColor = Color.blue;
    private float eatenAlphaValue = 0.2f;          // alfa que tendremos al ser comidos
    public float eatableTime;               // tiempo durante el cual sere comible
    private float currentTimeEatable;              // tiempo que ha pasado desde que he empezado a ser comible

    public float timeOnBase;                // timepo que estaremos en base
    private float currentTimeOnBase;         // tiempo que llevamos en base

	// Use this for initialization
	public virtual void Start () {

        // captura de referencias
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<MyGrid>();
        pacMan = GameObject.FindGameObjectWithTag("PacMan").GetComponent<PacMan>();

        ghostBaseObject = GameObject.FindGameObjectWithTag("GhostBase");

        //ghostRenderer = transform.GetComponentInChildren<Renderer>();

        // base de los fantasmas
        ghostBase = grid.GetNodeContainingPosition(ghostBaseObject.transform.position); // guardamos donde se encuentra la base
        ghostBaseObject.transform.position = ghostBase.position;        // ajustamos su posicion

        //  guardamos los materiales de los fantasmas
        // transform.GetComponentInChildren<Renderer>().material = initialMaterial;
  


        // DANI SI PUEDES HAZ QUE SEA LA ZONA ENTERA LA QUE SE CONSDIERE LA BASE Y QUE LOS FANTASMAS PUEDAN IR A DIFERENTES POSICIONES DE ESTA
        // Y QUE ADEMAS SE GUARDEN LAS COORDENADAS DE TODOS LOS NODOS SITUADOS AHI

        // waypoints
        currentWaypointIndex = 0;
        nodeToReach = waypoints[currentWaypointIndex].GetNode();   

        // guardamos el nodo en el que estamos
        currentNode = grid.GetNodeContainingPosition(transform.position);
        initialNode = currentNode;

        //Debug.LogError(currentNode == initialNode, gameObject);

        pathCalculated = false;
        // usamos su posicion para colocarnos bien
        grid.AdjustPosition(this.gameObject, currentNode);

        // creamos la lista para el camino
        pathToFollow = new List<Node>();            // si inicializamos eta variable en el start no sra accesible desde otras funciones que no sean de unity (CONJETURA)

        // guardamos el color inicial
        initialColor = ghostRenderer.material.color;


    }
	
	// Update is called once per frame
	public virtual void Update () {


        //Debug.LogWarning(currentNode + " " + initialNode);

        // si somos comibles iniciamos el timer que indicara el tiempo que somos vuldnreables a ser comidos
        if (eatable)
        {
            currentTimeEatable += Time.deltaTime;
            if (currentTimeEatable >= eatableTime)
            {
                eatable = false;
                ghostRenderer.material.color = initialColor;
                currentTimeEatable = 0f;
                movementType = MovementType.waypointMode;
            }
        }

        // si hemos sido comidos y estamos en la base iniciamos el timer que hara que el fantasma espere x tiempo en la base
        if (eaten && onBase)
        {


            // seteamos el fantasma como no movible
            canMove = false;
            // seteamos el material normal 
            ghostRenderer.material.color = initialColor;
            // augmentamos el contador de tiempo
            currentTimeOnBase += Time.deltaTime;

            // si hemos superado el tiempo de espera salimos de la base y volvemos a patrullar
            if (currentTimeOnBase >= timeOnBase)
            {
                //movementType = MovementType.waypointMode;       // volvemos al modo de waypoints
                movementType = MovementType.returningToGame;       // volvemos al modo de waypoints

                currentTimeOnBase = 0f;     // reseteamos el timer
                canMove = true;     // ya podremos movernos
                eaten = false;
                eatable = false;
                onBase = false;
                pathCalculated = false;
                currentNode = grid.GetNodeContainingPosition(transform.position);
                Debug.LogError("He salido del tiempo de la base");

            }
            
                 

        }


        if (canMove)
        {

            // guardamos el nodo en el que estamos
            //currentNode = grid.GetNodeContainingPosition(transform.position);

            // dependiendo del tipo de modo seleccionado nos moveremos de un modo u dde otro
            switch (movementType)
            {
                #region(Waypoint)
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                case (MovementType.waypointMode):


                    // estamos entrando todo el rato al if y recalculando al ruta lo cual impide sobrescirbe cosntantemente el camino que queremos tomar


                    // calculamos el camino unicamente cuando llegamos al centro del siguiente nodo
                    if (!pathCalculated)
                    {
                        pathToFollow = grid.FindPath(currentNode, waypoints[currentWaypointIndex].GetNode());
                        pathCalculated = true;
                        Debug.LogWarning("Calculando camino");


                        // si estamoa x distancia del jugador pasamos al modo de persecucion
                        if (grid.GetDistanceBetweenNodes(currentNode,pacMan.GetNode()) < maxChaseDistance)
                        {
                            movementType = MovementType.chase;
                        }

                        // debug del path to follow (comprobamos que estebien ordenador) 
                        /*
                        foreach (Node node in pathToFollow)
                        {
                            Debug.LogWarning("px :" + node.gridPosition_x + " py: " + node.gridPosition_y + " INDEX: " + pathToFollow.IndexOf(node));
                        }
                        */
                    } else
                    {

                        // realizamos el movimiento
                        Move();

                        //Debug.Log(nextNode);
                        // calculamos cuando llegamos al siguiente nodo
                        
                        if (transform.position == nextNode.position)
                        {
                            //Debug.LogWarning("reseteamos el calculo");
                            pathCalculated = false;
                            //Debug.Log(pathToFollow.Count);

                        }
                        
                    }


                    Debug.DrawLine(transform.position, currentNode.position, Color.yellow);


                    // si hemos llegado al waypoint de destino nos dirigimos hasta el siguiente
                    if (currentNode == waypoints[currentWaypointIndex].GetNode())
                    {
                        // nuestro nuevo nodo sera el del waypont al que hemos llegado
                        currentNode = waypoints[currentWaypointIndex].GetNode();

                        // activamos el flag de calcuolo de camino
                        pathCalculated = false;

                        // si nuestro waypointIndex es el ultimo dentro del array de waypoints volvemos al inicial
                        if (currentWaypointIndex == waypoints.Length - 1)
                        {
                            currentWaypointIndex = 0;
                        }
                        else // en caso contrario seguimos al siguiente nodo
                        {    
                            // vamos al siguiente waypont
                            currentWaypointIndex++;
                        }
                    }
                    break;

                #endregion
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                #region(chase)

                // cuando estamos en estado de persecucion
                case (MovementType.chase):

                    // guardamos la direccion del pacMan
                    Vector3 pacManDirection = pacMan.direction;
                    Node pacManNode = pacMan.GetNode();

                   // currentNode = grid.GetNodeContainingPosition(this.transform.position);
                    nodeToReach = grid.GetNode( (int)( pacManNode.gridPosition_x + (destinyOffset * pacManDirection.x)) ,(int)(pacManNode.gridPosition_y + (destinyOffset * pacManDirection.y)));

                    // si estamoa x distancia del jugador pasamos al modo de waypoints
                    if (grid.GetDistanceBetweenNodes(currentNode, pacMan.GetNode()) >= maxChaseDistance)
                    {
                        movementType = MovementType.waypointMode;
                    }

                    // generamos nuestra ruta hacia el player
                    pathToFollow = grid.FindPath(currentNode, nodeToReach);

                    // realizamos el movimiento
                    Move();

                    // En este momento el fantasma se dirigira a por el pacman (TODOS ACTUARAN IGUAL)
                    // Tendremos que usar un on triggerenter para matar al pacman y colocar el resto de elementos en su sitio


                    break;

                #endregion
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                #region(runAway)

                // este eastado se activara cuando el pacman consiga el elemento que hace que sea capaz de comerse al os fantasmas
                case (MovementType.runAway):

                    #region(Clever Scape)
                    /*
                    // calculamos la ruta de huida
                    // - direccion contraria y en definiitva que nos lleve mas lejos del player
                    // para esto miraremos nuestros vecinos y compararemos su distancia al player con la nuestra, si esta distancai es menor en su caso
                    // es porque nos alejamos del player

                    // guardamos nuestros vecinos en una lista
                    List<Node> neighbours = new List<Node>();
                    neighbours = grid.GetNeighnoursExtended(currentNode);
                    // creamos un array de ints que seran las distancias al pacman para no crear mas variables dentro de los nodos que nos guarden informacion tan superflua
                    //List<int> neigboursDistances = new List<int>();
                    int[] neighboursDitances = new int[neighbours.Count];

                    // iteramos por los vecinos calculando su distancia hasta el player y guardamos esta en la lista de ints
                    for (int i = 0; i < neighbours.Count; i++)
                    {
                        neighboursDitances[i] = grid.GetDistanceBetweenNodes(neighbours[i], pacMan.GetNode());
                    }
                    // calculamos nuestra distancia
                    distanceToPacman = grid.GetDistanceBetweenNodes(currentNode, pacMan.GetNode());

                    // sabiendo la distancia de los vecinos al pacman la comparamos con nuestra distancia al pacman, si la suya es menor usamos la suya
                    // en este punto trato de hacer la ia un poco tonta al no coger siempre la mejor ruta de escape
                    // pillamos el primero que vemos que esta mas lejos que nosotros del pacman
                    for (int i = 0; i < neighboursDitances.Length; i++)
                    {
                        // miramos que sea un nodo valido
                        if (neighbours[i].isTransitable)
                        {
                            // priorizamos ir al que este mas lejos
                            if (neighboursDitances[i] > distanceToPacman)
                            {
                                nodeToReach = neighbours[i];        // vamos al nodo del vecino que mas se aleja del pacman
                                break;
                            }
                            // en el caso que no haya nadie mas grande usamos el que sea mas grande o igual
                            else if (neighboursDitances[i] == distanceToPacman)
                            {
                                nodeToReach = neighbours[i];        // vamos al nodo del vecino que mas se aleja del pacman
                                break;
                            }
                        }
     
                    }

                    // calculamos la ruta hacia el punto de destino
                    pathToFollow = grid.FindPath(currentNode, nodeToReach);

                    // realizamos el movimiento
                    Move();
                    */

                    #endregion

                    // cuando huimos pasamos al modo de patrulla
                    movementType = MovementType.waypointMode;

                    break;

                #endregion
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                #region(eaten)
                case (MovementType.eaten):

                    // comprobamos si hemos llegado ya a la base
                    if (currentNode == ghostBase)
                    {
                        onBase = true;
                        currentNode = ghostBase;

                    } else
                    {

                        onBase = false;
                        // cuando un fantasma es comido este ha de volver de nuevo a la base, marcada como uno o varios nodos
                        nodeToReach = ghostBase;                                // vamos direccion a la base

                        // realizamos el movimiento
                        // solo calculamos una vez el camino
                        if (pathCalculated == false)
                        {
                            pathToFollow = grid.FindPath(currentNode, nodeToReach); // guardamos el camino que hemos de seguir
                            pathCalculated = true;
                        }

                        Move();
                    }



                    break;
                #endregion
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                case (MovementType.returningToGame):


                    // estado en el que entraremos al salir de la base
                    // si aun no hemos calculado el camino lo calculamos
                    if (!pathCalculated)
                    {

                        pathToFollow = grid.FindPath(currentNode, waypoints[currentWaypointIndex].GetNode());
                        Debug.LogWarning("llamando al update del calculo");
                        pathCalculated = true;

                    } else
                    {
 

                        Debug.DrawLine(currentNode.position, waypoints[currentWaypointIndex].GetNode().position);

                        if (currentNode == waypoints[currentWaypointIndex].GetNode())
                        {
                            pathCalculated = false;
                            movementType = MovementType.waypointMode;
                        }
                    }
                    Move();

                    break;
            }

        }


	}

    // funcion que controlara el movimiento del fantasma
    private void Move( )
    {

            // si hauy un camino calculado
            if (pathToFollow[1] != null )
            {
                // guardamos la posicion del siguiente nodo
                nextNode = pathToFollow[1];
            }
            else
            {
                return;
            }

            // realizamos el movimiento hacia el siguiente punto del camino
            transform.position = Vector3.MoveTowards(transform.position, nextNode.position, speed * Time.deltaTime);

            Debug.Log("Nos movemos");


            // si hemos llegado a ese punto recalculamos la ruta (para optimizar el calculo)
            if (transform.position == nextNode.position)
            {
                // setamos esta posicion como nuestra nueva posicion
                currentNode = pathToFollow[1];

            }




    }



    // funcion para reiniciar el nivel a las posiciones originasl tras pacman perder una vida
    public void ResetInitialPosition ()
    {
        currentNode = initialNode;

        transform.position = currentNode.position;

        canMove = false;
        eaten = false;
        eatable = false;

        // resetamos las variable spara el movimento en modo waypoint
        currentWaypointIndex = 0;
        nodeToReach = waypoints[currentWaypointIndex].GetNode();

    }



    // activamos o desactivamos el movimiento
    public void EnableOrDisableMovement(bool enabled)
    {
        canMove = enabled;
    }


    // funcion que iniciara los procedimientos de huida del fantasma
    public void InitiateRunAwayState ()
    {
        //Debug.Log(ghostRenderer.materials +
        //    " " + 
        //    runAwayMaterial);

        // el material del fantasma cambiara 
        Debug.Log (runAwayColor);
        Debug.Log(ghostRenderer);
        Debug.Log(ghostRenderer.material);
        Debug.Log(ghostRenderer.material.color);


        ghostRenderer.material.color = runAwayColor;  // runaway es null si lllamo a esta funcion
        

        // los marcaremos como comibles
        eatable = true;

        // cambiamos su patron de comportamiento a huida
        movementType = MovementType.runAway;

    }

    // funcion que llamaremos cuando queramos comernos al fantasma en cuestion (la llamamos directamente desde el pacman)
    public void BeEaten()
    {
        pathCalculated = false;

        // cambiamos el estado del fantasma para que vuleva a la base
        movementType = MovementType.eaten;

        // cambiamos el material a ser frenredizado como un material transparente
        ghostRenderer.material.SetFloat("_Mode", 3f);

        // cambiamos su alfa por la introducida por el usuario
        ghostRenderer.material.color = new Color(runAwayColor.r,
                                                runAwayColor.g,
                                                runAwayColor.b,
                                                eatenAlphaValue);

        // marcamos el objeto como comido
        eaten = true;
    }
    

    
}
