using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : PunctuableItem {




    public float score = 5000;        // puntuacion que nos dara el objeto
    public float timeBeforeDespawn = 10f;   // tiempo que tardara en desparecer tras su aparicion
    private float currentAliveTime;



    // hacemos que la puntuacion que nos vaya a devolver sea la seteada en este objeto pero usando la funcion
    // del padre
    protected override void Start()
    {
        base.Start();
        scoreInput = score;
        currentAliveTime = 0f;
    }

    private void Update()
    {
        // updatemao el timer de vida
        currentAliveTime += Time.deltaTime;

        // comprovamos que no haya pasado ya el tiempo maximo de vida
        if (currentAliveTime >= timeBeforeDespawn)
        {
            Destroy(this.gameObject);
        }

    }
}
