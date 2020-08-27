using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBall : PunctuableItem {

    public float score = 100;        // puntuacion que nos dara el objeto

    protected override void Start()
    {
        base.Start();
        scoreInput = score;

    }
}
