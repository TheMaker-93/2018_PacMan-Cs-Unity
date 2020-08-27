using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBall : PunctuableItem {


    public float score = 0;        // puntuacion que nos dara el objeto

    // hacemos que la puntuacion que nos vaya a devolver sea la seteada en este objeto pero usando la funcion
    // del padre
    protected override void Start()
    {
        base.Start();
        scoreInput = score;

    }

}
