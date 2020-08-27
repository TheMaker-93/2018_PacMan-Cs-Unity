using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

// caccedemos al editor del myGrid (inspector) y le añadimos funcionalidades
[CustomEditor(typeof(MyGrid))]
public class GridEditor : Editor {

    public Vector2 startingPoint;       // punto de inicio del calculo de path
    public Vector2 endPoint;            // punto final del calculo
    public MyGrid grid;

    // funcion que dibuja el inspector
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();      // dibuajmos el inspector tal y como lo teniamos

        DrawDefaultInspector();     // con esto llammos a la funcion de OnINspectorGUI de la base

        if (GUILayout.Button("Generate Grid Reference", GUILayout.Width(240)))
        {
            Debug.Log("Grid Creada");
            MyGrid grid = target as MyGrid;// target es una variable inerna que contiene el elemento que estamos pintadno ene l inspector (en este caso seria el gameobject que contiene el cidogo)

            // ahora ccon el codigo siguiente podremos generar la grid incuso antes de el moemnto de start

            grid.GenerateGrid();

        }

        if (GUILayout.Button("Order elements of the scene", GUILayout.Width(300)))
        {
            MyGrid grid = target as MyGrid;// target es una variable inerna que contiene el elemento que estamos pintadno ene l inspector (en este caso seria el gameobject que contiene el cidogo)

            grid.GenerateGrid();

            // funcion que colocara todos los elementos del juego en su posicion

            //Scene scene = SceneManager.GetActiveScene();
            //scene.GetRootGameObjects(allObjects);


            //// iterate root objects and do something
            //for (int i = 0; i < allObjects.Count; ++i)
            //{
            //    GameObject gameObject = allObjects[i];

            //    if (gameObject.tag == "BigBall" || gameObject.tag == "Ball")
            //    {
            //        PunctuableItem pItem = gameObject.GetComponent<PunctuableItem>();

            //        pItem.grid.AdjustPosition(gameObject, pItem.currentNode);
            //    }

            //}

            // iteramos por todos los gameobjets de la escena y dependeindod el tipo que sean lalmamaos a una u otra funcion
            //foreach (GameObject gO in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            //{
            //    if (gO.tag == "BigBall" || gO.tag == "Ball")
            //    {
            //        PunctuableItem pItem = gO.GetComponent<PunctuableItem>();

            //        pItem.grid.AdjustPosition(gO, pItem.currentNode);
            //    }


            //}

            // iteramos por todos los nodos del grid y llamamos a una funcion que nos dara el go del nodo . despues usaremos este go y lo pasaremos como parametero a grid adjsut positin
            foreach (Node node in grid.GetGrid() )
            {
                GameObject go =  grid.GetObjectOfNode(node);
                Debug.Log("hemos entrado en el foreach");
                // si hay algo ajustamos su posicion

                // ajustamos su posicion teniendo en cuenta que el objeto que colocaremos es el que hemos encontrado buscando en la posicion del nodo y que el nodo actual de este objeto es ese mismo nodo
                if (go != null && go.transform.tag != "OrderExeption")
                {
                    grid.AdjustPosition(go, node);
                }
   
                //grid.AdjustPosition(go, go.GetComponent<>)
            }


        }


        // VALORES QUE RELLENAREMOS DESDE EL INSPECTOR PARA CALUCLAR EL CAMINO
        EditorGUILayout.LabelField("Prueba del algrotimo de path finding");
        startingPoint.x = EditorGUILayout.FloatField("startingpoint x",startingPoint.x);
        startingPoint.y = EditorGUILayout.FloatField("startingPoint.y", startingPoint.y);
        endPoint.x =        EditorGUILayout.FloatField("endPoint.x", endPoint.x);
        endPoint.y  =EditorGUILayout.FloatField("endPoint.y", endPoint.y);

        // boton de debug para calcular caminos 
        if (GUILayout.Button("Calculate path between nodes",GUILayout.Width(240)))
        {
            MyGrid myGrid = target as MyGrid;
            // antes de asignar el grid lo generamos
            myGrid.GenerateGrid();

            Node [,] grid = myGrid.GetGrid();

            myGrid.FindPath(grid[(int)startingPoint.x,(int)startingPoint.y], grid[ (int)endPoint.x, (int)endPoint.y]);
        }

    }

}
