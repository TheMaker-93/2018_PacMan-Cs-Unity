using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    [Header("PlayerScore")]
    // la puntuacion la tiene guardara la clase estatica playerScore
    public Text scoreUIText;        // texto que mestra la puntuacion
    public float currentScore;      // puntuacion actual remitida desde la clase PlayerScore

	// Use this for initialization
	void Start () {

        currentScore = PlayerData.GetPlayerScore();
        scoreUIText.text = currentScore.ToString();     // cargamos la puntuacion que pueda estar guardara
	}
	

    // incrememtamnos la puntuacion del jugador y la actualizamos en el HUD
    public void IncreaseScore ( float delta)
    {
        PlayerData.IncreasePlayerScore(delta);                         // augmentamos la puntuacion del jugador
        currentScore = PlayerData.GetPlayerScore();
        scoreUIText.text = currentScore.ToString();                     // actualizamos la puntuacion del UI
    }

    

}
