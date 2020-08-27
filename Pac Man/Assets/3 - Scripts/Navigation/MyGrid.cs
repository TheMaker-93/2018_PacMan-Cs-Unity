using UnityEngine;
using System;
using System.Collections.Generic;

// objeto en la escena que gestinaara todos los cubos de la grid, 
// contenedor de una matriz de nodos

public class MyGrid : MonoBehaviour {

    public int size_x, size_y;    // tamaño de la grid
    public int node_Size = 1;
    private Node[,] grid;
    
    public Vector2 playableAreaMin;     // coordenada minima de juego delimitando el esquina inferior izquierda del terreno de juego
    public Vector2 playableAreaMax;     // coordenada maxima del juego delimtiando el esquina superior derecha

    // path a seguir
    public List<Node> pathToFollow = new List<Node>();  // lista donde guardaremos el camino a recorrer

    private void Awake()
    {
        GenerateGrid();

        //GetDistanceBetweenNodes(grid[1, 1], grid[3, 7]);
        // debug
        //Node startingNode = grid[4, 4];
        //Node endingNode = grid[12, 12];

        //FindPath(startingNode, endingNode);
 
    }

    public void GenerateGrid()
    {
        // RESERVAMOS ESPACIO EN MEMORIA
        grid = new Node[size_x, size_y];

        // GENERAMOS LOS NODOS
        // Iteramos por las columnas
        for (int i = 0; i < size_x; i++)
        {
            // itertamos por las hileras
            for (int j = 0; j < size_y; j++)
            {
                Vector3 nodePosition = new Vector3(node_Size * 0.5f + i * node_Size, node_Size * 0.5f + j * node_Size, 0);
                // una vez tenemos la seccion de memoria guardada generamos el objeto ya con sus datos
                // x e y (los seteamos llamando al constructor de Node)
                Vector3 wordlNodePosition = transform.position + nodePosition;

                // generamos un colliders y nos devuelve los colliders con los que colisiona
                Collider[] colliders = Physics.OverlapSphere(wordlNodePosition, node_Size * 0.5f);

                // cuando encontremos un elemento de tipo pieza ese nodo se tornara no transitable
                bool isTransitable = true;
                for (int k = 0; k < colliders.Length; k++)
                {
                    // si el nodo tiene un objeto de tag pieza marcamos ese nodo como no transitable
                    if ((colliders[k].tag == "Pieza"))
                    {
                        isTransitable = false;
                    }

                }

                // si colliders tiene algun collider es quehemos encontrado algun obj en esta posicio y por lo tanto no es transitable
                grid[i, j] = new Node(i, j, node_Size, wordlNodePosition , isTransitable); // la posicion en la grid mas la local
            }
        }
    }

    // funcion que ajustar ala posicion del objeto que le pasemos
    public void AdjustPosition (GameObject gObject, Node currentNode)
    {
        Debug.Log( "objeto " + gObject.transform.name + "ajsutado a la posicion de la grid: " + currentNode.gridPosition_x + " x " + currentNode.gridPosition_y +  " y ");

        gObject.transform.position = new Vector3(currentNode.position.x, currentNode.position.y, transform.position.z);
             // usamos su posicion para colocarnos bien
        //transform.position = new Vector3(currentNode.position.x, currentNode.position.y, transform.position.z);
    }

    // buscamos el objeto que se encuentra en la posicion de este nodo
    public GameObject GetObjectOfNode (Node node)
    {

        // buscamos los elementos que puedan existir en esta posicion
        Collider[] colliders = Physics.OverlapSphere(node.position, node_Size * 0.5f);


        for (int i = 0; i < colliders.Length; i++)
        {

            GameObject gOtoReturn = colliders[i].gameObject;
            return gOtoReturn;
        }

        Debug.LogWarning("No se ha encontrado ningun elemento en " + node.gridPosition_x + " " + node.gridPosition_y);

        return null;

        

       
    }


    // MOSTRAMOS LOS ELEMENTOS DE LA GRID EN PANTALLA
    public void OnDrawGizmos()
    {
        // si hay grid
        if (grid != null)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    //Vector3 position = new Vector3(node_Size/2 + i *node_Size, node_Size/2 + j*node_Size, 0);
                    Vector3 scale = new Vector3(node_Size, node_Size, node_Size);

                    // si no es transitable lo pintamos rojo
                    if (!grid[i,j].isTransitable)
                    {
                        
                        Gizmos.color = new Color(255, 0, 0, 0.25f);
                        Gizmos.DrawCube(grid[i, j].position, scale);
                        

                    } else
                    {

                        Gizmos.color = new Color(0, 255, 0, 0.075f);     // color verde por defecto
                        Gizmos.DrawWireCube(grid[i, j].position, scale);

                        

                    }
                    /*
                    
                    if (grid[i, j].ghostUsingThisNode != null)
                    {
                        // guardamos el fantasma que pueda esta uranso este nodo
                        string ghostName = grid[i, j].ghostUsingThisNode;

                        if (ghostName == "Pinky")
                        {
                            Gizmos.color = Color.cyan;

                        }
                        else if (ghostName == "Clyde")
                        {
                            Gizmos.color = Color.cyan;

                        }
                        else if (ghostName == "Blinky")
                        {
                            Gizmos.color = Color.red;

                        }
                        else if (ghostName == "Inky")
                        {
                            Gizmos.color = Color.blue;

                        }
                        else
                        {
                            
                            Debug.LogError("Nombre No encontrado");

                        }
                        Gizmos.DrawCube(grid[i, j].position, scale);
                        
                        /*
                        if (grid[i, j].pathNode)
                        {
                            Gizmos.color = new Color(255, 255, 255, 0.25f);
                            Gizmos.DrawCube(grid[i, j].position, scale);
                            continue;
                        }

                        
                        
                        //

                        // seteamos el nodo del color que este tenga
                        Gizmos.color = new Color(grid[i, j].nodeColor.r, grid[i, j].nodeColor.g, grid[i, j].nodeColor.b, grid[i, j].nodeColor.a);
                        Gizmos.DrawCube(grid[i, j].position, scale);
                        


                    }
                    
                    */




                }
            }
        }   
    }

    // esta funcion nos dira que nodo esta en esa posicion que le pasamos
    public Node GetNodeContainingPosition(Vector3 wordlPosition)
    {
        // guardamos la posocion que nos pasan respecto a la grid
        Vector3 localPosition = wordlPosition - transform.position;

        // respecto a las locales devolvemos el nodo que contiene
        int x = Mathf.FloorToInt( localPosition.x / node_Size);
        int y = Mathf.FloorToInt( localPosition.y / node_Size);

        // para que nos devuelva un nodo si este esta dentro del rango del array
        if (x < size_x && x >= 0 && y < size_y && y >= 0 )
        {
            return grid[x, y];
        } else
        {
            // en el caso que este fuera de la zona de la grid devolvemos null
            Debug.LogError("El nodo al que intentas acceder no esta dentro del rango posible de la matriz GRID");
            return null;
        }

        
    }

    // crea una funcion que busque un nodo en una posicion de la grid concreta
    /*
    public Node GetNodeOfGrid(Vector2 positionInGrid)
    {
        // guardamos la poscion en unas variables intertnas (facilidad de lectura)
        int x = Mathf.FloorToInt(positionInGrid.x );
        int y = Mathf.FloorToInt(positionInGrid.y );

        // si estamos dentro del ragon posible de la matriz de grid
        if (x < size_x && x >= 0 && y < size_y && y >= 0)
        {
            return grid[x, y];
        } else
        {
            Debug.LogError("El nodo al que intentas acceder no esta dentro del rango posible de la matriz GRID");
            return null;
        }
    }
    */
    // devolvemos un node en x e y posicion del grid 
    public Node GetNode(int x, int y)
    {
        // devolvemos nulo si el nodo no esta en lla tabla
        if ( x < 0 || y < 0 || x > size_x || y > size_y)
        {
            Debug.LogWarning("Se ha pedido un nodo no valido en la posicion (" + x + "," + y + ")");
            return null;
        }

        return grid[x, y];
    }

    // devolvemos los vecinos en una lista (8 vecinos) 
     public List<Node> GetNeighnoursExtended(Node node, bool extended = false)
    {
        // guardamos el epscio para una lista de nodos
        List<Node> listToReturn = new List<Node>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1;  j <= 1;  j++)
            {
                if (!extended)
                {
                    // si son las diagonales ignorammos el caluclo si no somos la version extendida
                    if (Mathf.Abs(i) == Mathf.Abs(j))
                    {
                        continue;
                    }
                }
                // si somos nosotros no guardamos el vecino
                else if (i == 0 && j == 0)
                {
                    continue;
                }

                // accedemos a la grid en la posicion calculada y guardamos el nodo que contiene
                Node vecino = GetNode(node.gridPosition_x + i, node.gridPosition_y + j);
                //Debug.LogWarning(vecino.gridPosition_x + " " + vecino.gridPosition_y);
                // añadimos el vecino a la lista
                listToReturn.Add(vecino);       

            }
        }

        return listToReturn;
    }

    // FUNCION PARA LA BUSQUEDA DEL CAMINO pseudo a +
    // OPTIMIZA ESTE CODIGO PARA EVITAR EL CALCULO DOBLE DE F G Y H CUANDO ES VECINO A CUANDO ES ELEMENTO ACTIVO AL INICIO DEL WHILE
    public List<Node> FindPath(Node _startNode, Node _endNode)
    {
        // debug
        if (_endNode == null )
        {
            Debug.LogError("Input nulo");
        }

        // variables iniciales
        Node startNode = _startNode;                    // nodo inicial
        Node endNode = _endNode;                        // nodo final

        // EXTRA // Comprobamos que los nodos no esten fuera dle area de juego (no comprobamos si estamos fuera de los nodos sino si esamos en la zona donde ponemos jugar)
        if (startNode.gridPosition_x < playableAreaMin.x || startNode.gridPosition_y < playableAreaMin.y ||
            endNode.gridPosition_x > playableAreaMax.x || endNode.gridPosition_y > playableAreaMax.y)
        {
            Debug.LogError("El nodo al que quieres viajar esta fuera del area de juego, CANCELANDO EJECUCION DEL PATHFINDIN");
            return null;
        }

        // DEBUG ////////////////////////////////////////////////////////
        //startNode.startOrEnd = true;
        //endNode.startOrEnd = true;
        ////////////////////////////////////////////////////////////////

        // lista de nodos
        List<Node> openNodes = new List<Node>();        // lista con los nodos abiertos
        List<Node> closedNodes = new List<Node>();      // lista de nodos ya cerrados

        // añadimos el nodo de inicio a la lista de abiertos
        openNodes.Add(startNode);

        // mientras que tengamos nodos abiertos
        while (openNodes.Count > 0)
        {
            // calculamos y guardamos las variables g f y h de los nodos abiertos
            for (int nodeId = 0; nodeId < openNodes.Count; nodeId++)
            {
                Node currentNode = openNodes[nodeId];                               // nodo al cual le estamos calculando las variables
                //currentNode.g = GetDistanceBetweenNodes(currentNode, startNode);    // distancia entre el nodo abierto y el inicial
                //currentNode.h = GetDistanceBetweenNodes(currentNode, endNode);      // distancia entre este nodo y el destino
                //currentNode.f = currentNode.g + currentNode.h;                      // variable de dificultad
                CalculateAStarVariables(currentNode, startNode, endNode);
                // debug
                currentNode.calculated = true;
            }

            // cogemos el nodo conla f mas baja o en caso que todas sean iguales con la f mas baja y la h mas baja
            Node lowestValueNode = GetLowestCostNode(openNodes);

            // cambiamos el nodo con el menor valor a cerrado (lo sacamos de los abiertos y lo metemos en los cerrados)
            openNodes.Remove(lowestValueNode);      // lo sacamos de los abiertos
            closedNodes.Add(lowestValueNode);       // lo metemos en los cerrados

            // si este es el ultimo nodo acabamos
            if (lowestValueNode == endNode)
            {
                // vaciamos la lisa antes de guardar en ella las nuevas posiciones
                pathToFollow = new List<Node>();

                // devolvemos la lista conteniendo el camino a recorrer 
                GetPath(endNode, ref pathToFollow,endNode,startNode);

                // DEBUG VISUAL
                // ITERAMOOS POR LA LISTA PARA MARCAR SUS CASILLAS COMO PUNTOS DE RUTA
                foreach (Node n in pathToFollow)
                {
                    n.pathNode = true;
                }

                // la invertimos para que podamos avanzar poe ella
                pathToFollow.Reverse();

                Debug.LogWarning("Path to follow relleno");

                break;


            } else   // si no hemos acabado buscamos todos los vecinos
            {
                // creamos una lista vacia para guardar a los vecinos
                List<Node> neighbours = new List<Node>();
                // guardamos los vecinos (arriba, debajo, derecha e izquierda)
                neighbours = GetNeighnoursExtended(lowestValueNode, false);

                // iteramos por los vecinos
                foreach (Node neighbour in neighbours)
                {
                    //Debug.Log(neighbours.Count);
                    //Debug.DrawLine(lowestValueNode.position, neighbour.position);

                    //Debug.Log("is transitable " + neighbour.isTransitable);
                    //Debug.Log(" " + closedNodes.Contains(neighbour));

                    // si no es transitable o esta cerrado
                    if ( neighbour.isTransitable == false || closedNodes.Contains(neighbour))
                    {
                        // continuamos con el bucle buscando un vecino optimo
                        continue;
                    }

                    // si es transitable y no esta en la lista de nodos cerrados seguimos

                    // calculamos la nueva g para compararla con el antigua a continuacion
                    int newG = GetDistanceBetweenNodes(neighbour, startNode);

                    // si no lo hemos visitiado aun  o si su nuevo coste de G es menor al que tenia anteriormente
                    if (!openNodes.Contains(neighbour) && !closedNodes.Contains(neighbour)   || newG < neighbour.g)     // COMPRACION DE INT CON NULL ES FALSE
                    {
                        // calculamos sus nuevas variables A ESTRELLA
                        CalculateAStarVariables(neighbour, startNode, endNode);
                        // si no esta en la lista de nodos abiertos lo añadimos
                        openNodes.Add(neighbour);
                        // guardamos como padre el ndo que lo descubrio
                        neighbour.parent = lowestValueNode;
                        
                        // DEBUG
                        //lowestValueNode.pathNode = true;
                        Debug.DrawLine(lowestValueNode.position, neighbour.position,Color.yellow);
                    }
                }
            }
        }

        return pathToFollow;
    }

    // nos devuleve un numedro int que sera la distancia entre dos nodos
    public int GetDistanceBetweenNodes (Node _startNode, Node _endNode)
    {
        // distancia en x
        int value = 
            (Math.Abs(_endNode.gridPosition_x - _startNode.gridPosition_x)
            +
            (Math.Abs(_endNode.gridPosition_y - _startNode.gridPosition_y)));

        //Debug.Log("la distancia entre " + _startNode.gridPosition_x + "." + _startNode.gridPosition_y + "  Y  " + _endNode.gridPosition_x + "." + _endNode.gridPosition_y + " es: " + value);

        return value;

    }

    public void CalculateAStarVariables (Node currentNode, Node startNode, Node endNode)
    {
        currentNode.g = GetDistanceBetweenNodes(currentNode, startNode);    // distancia entre el nodo abierto y el inicial
        currentNode.h = GetDistanceBetweenNodes(currentNode, endNode);      // distancia entre este nodo y el destino
        currentNode.f = currentNode.g + currentNode.h;                      // variable de dificultad
    }

    // nos devuelve el camino a recorrer entrando en cada padre de la lista 
    // en currentNode pasamos el nodo final (objetivo) y en el pathtofollow la lista que queremos llenar con el camino
    /*
    public void GetPath (Node currentNode, ref List<Node> pathToFollow)
    {
        // si el nodo actual tiene padre :
        if (currentNode.parent != null)
        {
            // HAY UN PROBLEMA GRAVE CON LOS INDICES EN ESTE SECTOR DEL JUEGO

            pathToFollow.Add(currentNode.parent);
            //Debug.LogError("capturado punto de ruta " + currentNode.gridPosition_x + " " + currentNode.gridPosition_y + " INDEX: " + pathToFollow.IndexOf(currentNode));
            GetPath(currentNode.parent,ref pathToFollow);

        } else
        {
            Debug.LogWarning("hemos llegado al inicio");

            // mostramos el debug del camino
            foreach (Node node in pathToFollow)
            {
                Debug.LogWarning("EN GRID px :" + node.gridPosition_x + " py: " + node.gridPosition_y + " INDEX: " + pathToFollow.IndexOf(node));
            }

            return;
        }
 
    }
    */
    public void GetPath(Node currentNode, ref List<Node> pathToFollow, Node destinyNode, Node startNode)
    {

        // guardamos este nodo si se trata del nodo final
        if (currentNode == destinyNode)
        {
            pathToFollow.Add(currentNode);
        }

        // si el nodo actual tiene padre y ademas el nodo que estamos escaneando no es el primero de todos
        if (currentNode.parent != null && currentNode != startNode)
        {
            pathToFollow.Add(currentNode.parent);
            GetPath(currentNode.parent, ref pathToFollow, destinyNode, startNode);

        }
        else
        {
            return;
        }

    }

    // retornamos el nodo con menor coste

    /*// ESTA FUNCION NECESITA UN BUEN REPASO COMPRUEBA QUE SU FUNCIONAMIENTO SEA CORRECTO
private Node GetLowestCostNode (List<Node> openNodes)
{
    Node lowestCostNode = new Node();       // nodo con el menor coste
    lowestCostNode.f = Mathf.Infinity;      // su f tendra valor infinito


    for (int i = 0; i < openNodes.Count; i++)                   // id del nodo que usamos de plantilla para la comparacion
    {
        for (int j = 0; j < openNodes.Count; j++)               // nodo con el que estamos comparando
        {
            if (openNodes[i] != openNodes[j])                   // si no nos estamos comparando con nosotros mismos
            {
                //////////////////////////////////////////
                // si ningun nodo es mas pequeño que yo yo sere el minimo F
                if (openNodes[i].f <= openNodes[j].f && 
                    openNodes[i].f <= lowestCostNode.f)
                {

                    // comprobamos el h
                    // si por lo que fuera la f de I es menor o igual entonces tendriamos un empate
                    // si la h de i es menor este sera el menor coste
                    if (openNodes[i].f == openNodes[j].f)
                    {
                        if (openNodes[i].h <= openNodes[j].h)
                        {
                            lowestCostNode = openNodes[i];
                        }
                        else if (openNodes[j].h <= openNodes[i].h)
                        {
                            lowestCostNode = openNodes[j];
                        }
                    } else
                    {
                        // siu las f no son iguales
                        // en el caso que ningun numero sea inferior a mi yo sere el numero inferior
                        lowestCostNode = openNodes[i];
                    }


                    // si el nodo j con el que comparo es menor que yo este sera el minimo
                } else if (openNodes[j].f <= openNodes[i].f &&
                    openNodes[j].f <= lowestCostNode.f)
                {

                    // comprobamos el h
                    // si por lo que fuera la f de I es menor o igual entonces tendriamos un empate
                    // si la h de i es menor este sera el menor coste
                    if (openNodes[i].f == openNodes[j].f)
                    {
                        if (openNodes[j].h <= openNodes[i].h)
                        {
                            lowestCostNode = openNodes[j];
                        }
                        else if (openNodes[i].h <= openNodes[j].h)
                        {
                            lowestCostNode = openNodes[i];
                        }
                    } else
                    {
                        lowestCostNode = openNodes[j];
                    }
                }
                //////////////////////////////////////////
            }
        }
    }

    // al usar mathf.infinite el nodo que se devovlera siempre sera valido
    return lowestCostNode;

}
*/

    // le pasamos una lista y nos devuelve el nodo con menor f y en el caso que haya empate comprobamos la h
    private Node GetLowestCostNode (List<Node> openNodes )
    {
        Node lowestValueNode = openNodes[0];        // determinamos que por defecto el primer valor de la lista sera el minimo

        for (int i = 0; i < openNodes.Count; i++)
        {
            for (int j = 0; j < openNodes.Count; j++)
            {
                // comprobamos que no nos estemos comparando con nosotros mismos
                if (i != j)
                {

                    if (openNodes[i].f < openNodes[j].f && openNodes[i].f < lowestValueNode.f)            // si la i es mayor a la j
                    {
                        lowestValueNode = openNodes[i];

                    } else if (openNodes[j].f < openNodes[i].f && openNodes[j].f < lowestValueNode.f)     // si la j es mayor a la i
                    {
                        lowestValueNode = openNodes[j];

                    } else if (openNodes[i].f == openNodes[j].f && 
                        openNodes[i].h <= lowestValueNode.h ||
                        openNodes[j].h <= lowestValueNode.h)    // si ambos son iguales
                    {
                        // comprobamos el h ahora
                        if (openNodes[i].h < openNodes[j].h)
                        {
                            lowestValueNode = openNodes[i];

                        } else if (openNodes[j].h < openNodes[i].h)
                        {
                            lowestValueNode = openNodes[j];

                        } else if (openNodes[i].h == openNodes[j].h)
                        {
                            lowestValueNode = openNodes[i];
                        }
                    }


                }


            }
        }

        return lowestValueNode;

    }

    // funcion para tomar el grid y poder llevar acabo el simulacro de calculo de camino
    public Node[,] GetGrid ()
    {
        return grid;
    }

}
