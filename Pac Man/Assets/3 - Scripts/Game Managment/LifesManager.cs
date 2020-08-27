using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifesManager : MonoBehaviour {

    private GameManager gManager;
    public List<GameObject> lifesObjects;
    public int lifesLeft;


    private void Awake()
    {
        gManager = GetComponent<GameManager>();
    }

    // Use this for initialization
    void Start () {

        Debug.Log("Vida jugador desde lifesManagr: " + PlayerData.playerLifesLeft);

        lifesLeft = PlayerData.playerLifesLeft;

        if (lifesLeft == 2)
        {
            Destroy(lifesObjects[lifesObjects.Count - 1]);
        } else if (lifesLeft == 1)
        {
            Destroy(lifesObjects[lifesObjects.Count - 1]);
            Destroy(lifesObjects[lifesObjects.Count - 2]);
        }


    }

    private void Update()
    {
    }

    // funcio que llamamos cuando nos impacta un fantasma
    public void ReduceLifesLeft (int delta = 1)
    {


        // reducimos sus vidas
        PlayerData.DecreaseLifes(delta);

        // quitamos el ultimo objeto y lo destruimos si quedan objetos en la lista de objetos
        if (lifesObjects.Count > 0)
        {
            Destroy(lifesObjects[lifesObjects.Count - 1]);
            lifesObjects.Remove(lifesObjects[lifesObjects.Count - 1]);
        }

        Debug.LogError(PlayerData.LifesAvalible());

        // si ya no nos quedan vidas cambiamos el estado del juego
        if (!PlayerData.LifesAvalible())
        {
            // pasamoas al estado de fin de juego cuando morimos
            gManager.ChangeStatusToEndgame(false);

            Debug.LogError("Cambiamos el estado del juego a final");

        } else
        {
            // si aun nos quedan volvemos al pregame
            gManager.GoToPreGame();

        }
    }
	

}
