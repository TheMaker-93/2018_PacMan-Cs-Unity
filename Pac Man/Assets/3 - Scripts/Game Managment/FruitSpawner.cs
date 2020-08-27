using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour {

    public MyGrid grid;
    public GameObject fruitPrefab;
    public float timeBeforeSpawn = 30f;
    public float timePassed;

	
	// Update is called once per frame
	void Update () {

        timePassed += Time.deltaTime;

        if (timePassed >= timeBeforeSpawn)
        {
            GameObject fruit =  Instantiate(fruitPrefab);
            // le asignamos un nodo aleatorio que sea transitable 
            PunctuableItem fruitController = fruit.GetComponent<PunctuableItem>();
            // mientra que la fruta no tenga un nodo o este sea intransitable le buscaremos uno
            // FALTA HACER QUE SOLO PUEDA APARECER DENTRO DEL TERRENO DE JUEGO Y NO POR ENCIMA O POR DEBAJO
            while (fruitController.currentNode == null || 
                fruitController.currentNode.isTransitable == false || 
                fruitController.currentNode.gridPosition_x < grid.playableAreaMin.x || fruitController.currentNode.gridPosition_y < grid.playableAreaMin.y ||
                fruitController.currentNode.gridPosition_x > grid.playableAreaMax.x || fruitController.currentNode.gridPosition_y > grid.playableAreaMax.y)
            {
                fruitController.currentNode = grid.GetNode(Random.Range(0, grid.size_x - 1), Random.Range(0, grid.size_y - 1));
            }

            // una vez ya tenemos un nodo transitable le decimos que la posicion de la fruta sera la de ese nodo
            fruit.transform.position = fruitController.currentNode.position;
            // le asignamos el grid a su variable grid
            fruitController.grid = grid;

            // resetamos el timer
            timePassed = 0f;

            //Debug.LogError("Hemos instanciado una fruta en el nodo " + fruitController.currentNode,gameObject);
        }

	}
}
