using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class SoundManager : MonoBehaviour {

    public GameObject masterSpeaker;        // altavoz principal para los sonidos de gameStatus
    public AudioSource aSource;

    [Header ("Sounds")]
    public AudioClip preLevelSound;         // sonido inicial
    private float timeLeftOfIntroduction;   // tiempo restante para que se acabe la fase de introduccion
    public float levelStartSoundDuration;  // duracion del sonido inicial (mas optimo que preguntarle si ha acabado o no)

	// Use this for initialization
	void Awake () {

        // master speaker
        aSource = masterSpeaker.GetComponent<AudioSource>();

        // guardamos la duracion del clip inicial
        levelStartSoundDuration = preLevelSound.length;

    }
	


    // funcion para reproducir un sonido
    public void PlaySound (AudioClip audioClip)
    {
        aSource.clip = audioClip;       // cargamos el clip que le pasamos
        aSource.Play();
    }
    public void StopSound ()
    {
        aSource.Stop();
    }

    // lazamientod el sonido inicial ded juego y retorno de si ha caccabado o no
    public bool LaunchInitialGameSound ()
    {
        // si no hay sonido sonando hacemos que suene
        
        if ( !aSource.isPlaying)
        {
            // devolveremos si ha acabado
            timeLeftOfIntroduction = preLevelSound.length;
            PlaySound(preLevelSound);
            timeLeftOfIntroduction -= Time.deltaTime;

            //StartCoroutine(StopMusic(timeLeftOfIntroduction, aSource));
            return true;

        } else
        {

            timeLeftOfIntroduction -= Time.deltaTime;
            if (timeLeftOfIntroduction <= 0f)
            {
                aSource.Stop();
              return false;
            }

            return true;

            //return false;

        }
        
        
        /*
        // si el sonido no esta sonando lo ejecutamos
        if (aSource.isPlaying == false)
        {
            PlaySound(preLevelSound);
            timeLeftOfIntroduction = preLevelSound.length;

            StartCoroutine(StopMusic(timeLeftOfIntroduction,aSource));

            return true;

        } else
        {
            return false;
        }
        */
    }

    IEnumerator StopMusic (float timeBeforeStop , AudioSource aSource)
    {

        yield return new WaitForSeconds(timeBeforeStop);

        StopSound();

        Debug.Log("Musica inicial detenida");
    }

}
