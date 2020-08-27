using UnityEngine;

public class Node  {

    public int gridPosition_x;
    public int gridPosition_y;
    public int size;
    public Vector3 position;            // posicion del nodo en el espacio
    public bool isTransitable;

    [Header("Path finding")]
    public float g;               // distancia al origen
    public float h;               // distancia al destino
    public float f;               // g + h
    public Node parent;         // padre del calculo
    public bool parented;       // controlamos si estamos o no emparentados

    // debug
    public bool calculated = false;
    public bool startOrEnd = false;
    public bool pathNode = false;

    // fantasma que esta usando este camino
    public string ghostUsingThisNode;

    // color
    //public Color nodeColor = Color.red;


    public Node()
    {
        // constructor vacio
        //SetTransparentColor();
    }

    // constructor
    // posicion en la x del grid , posicion en la y, tamaño, posicion en el mundo, se puede transitar en el ?
    public Node(int _gridPosition_x, int _girdPosition_y, int _size, Vector3 _position, bool _isTransitable)
    {
        gridPosition_x = _gridPosition_x;   
        gridPosition_y = _girdPosition_y;
        size = _size;
        position = _position;
        isTransitable = _isTransitable;

        //Debug.Log(gridPosition_x + " " + girdPosition_y);
        //SetTransparentColor();


    }

    /*
    // seteamos el camino del color de nuestro fantasma
    public void SetNodeColor (Color color)
    {
        nodeColor = color;
    }

    // resetamos el color
    public void SetTransparentColor ()
    {
        nodeColor = new Color(0,0,0,0); 
    }

    */
}
