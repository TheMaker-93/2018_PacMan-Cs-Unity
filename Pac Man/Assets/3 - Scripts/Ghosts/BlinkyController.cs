using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkyController : GhostController {

	// Use this for initialization
	public override void Start () {

        // aqui definitiarmos los parametros de IA que queremos que sean diferentes en este fantasma
        // Rojo actua del modo por defecto con lo cual no sufrira ninguan modificacion a su comportamiento



        // ejecutamos el padre
        base.Start();

	}
	
	// Update is called once per frame
	public override void Update () {

        // ejecutamos la funcion Update del padre ya que es la que hace toda la faena
        base.Update();

        // para cada nodo del camino seteamos su color al del fantasma
        //PaintPathNodes();
        //PaintPathNodes(Color.blue);

        // marcaremos los nodos como nuestros 
        foreach (Node node in pathToFollow)
        {
            node.ghostUsingThisNode = " Blinky";
        }

    }
}
