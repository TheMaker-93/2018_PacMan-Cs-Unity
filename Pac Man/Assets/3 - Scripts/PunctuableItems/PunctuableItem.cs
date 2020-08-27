using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class PunctuableItem : MonoBehaviour {

    [Header("pointsPerItem")]
    public float smallBallScoreInput = 100f;
    public float bigBallScoreInput = 5000;

    private float scoreInput;       // puntuacion que añadira a quien lo consuma
    public enum PunctuableItemType
    {
        smallBall, bigBall
    }
    public PunctuableItemType itemType;

    private void Start()
    {
        switch (itemType)
        {
            case (PunctuableItemType.smallBall):
                scoreInput = smallBallScoreInput;
                break;
            case (PunctuableItemType.bigBall):
                scoreInput = bigBallScoreInput;
                break;
        }
    }


    // extraemos el score y generamos el efecto dependiendo del tipo de objeto
    // POSIBLEMENTE USAREMOS ESTA CLASE PARA LOS FANTASMAS EN ESTADO DE HUIDA
    public float GetScore ()
    {
        // generamos el sonido


        // retornamos la puntuacion ( el player es quien nos destruye)
        return scoreInput;
    }

}
*/
public class PunctuableItem : MonoBehaviour
{
    [HideInInspector] public float scoreInput;            // puntuacion que recibiremos
    public Node currentNode = new Node();            // nodo en el que estamos situados
    public MyGrid grid;                 // grid del juego

    public GameObject soundGenerator;   // prefab del generador de sonido
    public AudioClip catchClip;         // efecto sonoro al coger este objeto

    // DANI ATENTO A ESTA FUCNIO YA QUE NO SE LLAMARA A NO SER QUE LA HAGAS VIRTUAL Y EL RESTO DE STARTS OVERRIDEN Y LLAMANDO A ESTA FUNCION
    protected virtual void Start()
    {
        //Debug.Log(transform.position + " " + grid);

        // ajustamos la posicion de este objeto al nodo en el que esta
        // guardamos el nodo en el que estamos

        currentNode = grid.GetNodeContainingPosition(transform.position);

       //AdjustPosition();

        grid.AdjustPosition(this.gameObject, currentNode);
        
    }


    public float GetScore()
    {

        if (catchClip != null)
        {
            // generamos el sonido
            ReproduceSFX();
        }


        // retornamos la puntuacion ( el player es quien nos destruye)
        return scoreInput;
    }

    private void ReproduceSFX ()
    {
        GameObject sg =  Instantiate(soundGenerator);
        sg.GetComponent<AudioEffectController>().PlayAudioclip(catchClip);
    }

    public void AdjustPosition ()
    {
        // usamos su posicion para colocarnos bien
        transform.position = new Vector3(currentNode.position.x, currentNode.position.y, transform.position.z);
    }

}