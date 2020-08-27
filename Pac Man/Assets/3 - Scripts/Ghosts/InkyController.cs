using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkyController : GhostController {

   

	// Use this for initialization
	public override void Start () {



        base.Start();

	}
	
	// Update is called once per frame
	public override void Update () {

        base.Update();
        // para cada nodo del camino seteamos su color al del fantasma
        //PaintPathNodes();

        // marcaremos los nodos como nuestros 
        foreach (Node   node in pathToFollow)
        {
            node.ghostUsingThisNode = " Inky";
        }

    }
}
