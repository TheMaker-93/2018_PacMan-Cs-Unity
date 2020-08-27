using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

    Node currentNode;
    public MyGrid grid;

	// Use this for initialization
	void Start () {
 
        // guardamos el nodo en el que estamos
        currentNode = grid.GetNodeContainingPosition(transform.position);
        // usamos su posicion para colocarnos bien
        //transform.position = new Vector3(currentNode.position.x, currentNode.position.y, transform.position.z);

        grid.AdjustPosition(this.gameObject, currentNode);

    }
	

}
