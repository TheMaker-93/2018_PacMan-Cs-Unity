using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ESTE OBJETO GUARDARA LA POSICION EN LA QUE ESTA Y EL NODO QUE LO CONTIENE PARA QUE EL FANTASMA PUEDA GUARDAR ESTE OBJETO JUNTO A LAS COORDENASDA 
// POR LAS QUE SE MOVERA

public class Waypoint : MonoBehaviour {

    private Node currentNode;        // nodo actual
    // debug
    public Vector2 currentNodePositionOnGrid;

        //

    public MyGrid grid;             // referencia al grid

	// Use this for initialization
	void Start () {

        // guardamos el nodo en el que nos encontramos
        currentNode = grid.GetNodeContainingPosition(transform.position);
        // ajustamos nuestra posicion ("no es necesario pero me es mas sencillo a la hora de trabajar en el viewport")
        grid.AdjustPosition(this.gameObject, currentNode);

        currentNodePositionOnGrid = new Vector2((int)currentNode.gridPosition_x, (int)currentNode.gridPosition_y);

    }


    public Node GetNode()
    {
        if (currentNode != null)
        {
            return currentNode;
        } else
        {
            //Debug.LogError("El nodo no existe actualmente", gameObject);
            return null;
        }
    }

}
