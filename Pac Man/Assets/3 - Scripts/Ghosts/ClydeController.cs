﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClydeController : GhostController {

	// Use this for initialization
	public override void Start () {



        base.Start();

	}
	
	// Update is called once per frame
	public override void Update () {

        base.Update();

        //Debug.Log(pathToFollow.Count);

        //PaintPathNodes();

        // marcaremos los nodos como nuestros 
        foreach (Node node in pathToFollow)
        {
            node.ghostUsingThisNode = " Clyde";
        }

    }
}
