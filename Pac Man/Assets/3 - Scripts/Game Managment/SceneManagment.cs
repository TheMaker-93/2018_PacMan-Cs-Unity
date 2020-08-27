using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagment : MonoBehaviour {

    Scene currentScene;     // escena actual
    public int currentLevelIndex;

    // Use this for initialization
    void Awake () {

        // guardamos la referencia al nivel actual
        currentScene = SceneManager.GetActiveScene();
        currentLevelIndex = currentScene.buildIndex;

    }
	

    // funcion llamada externamente que cargara el siguiente nivel si este existe
    // en el caso que no exista, lo creamos
    public void LoadNextLevel ()
    {

       try
        {
            // tratamos de cargar el siguiente nivel si este existe
            SceneManager.LoadScene(currentLevelIndex + 1);

        } catch
        {
            Debug.LogError("La escena con indice " + currentLevelIndex + 1 + " no existe o no esta asignada en los build settings");
        }


    }

    // creado por si a caso pero jamas repetiremos un nivel
    public void ReloadCurrentLevel()
    {
        SceneManager.LoadScene(currentLevelIndex);
    }
}
