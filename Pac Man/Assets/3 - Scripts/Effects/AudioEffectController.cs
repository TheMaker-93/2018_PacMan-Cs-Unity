using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// VERSION 1.0 
// - sonido autonomo para espacios 2d/3d
// - Posibilidad de seguimiento


// Autor_ Daniel Moreno Bernal          <The_Maker>

    [RequireComponent(typeof (AudioSource))]

public class AudioEffectController : MonoBehaviour {

    [Header("Sound")]
    public AudioSource audioSource;     // audiosource del objeto ASIGNADO DESDE EL INSPECTOR para no empeorar el rendimiento
    public GameObject toFollow;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // setters
    public void PlayAudioclip(AudioClip audioEffect, GameObject toFollow = null)
    {
        audioSource.clip = audioEffect;
        audioSource.Play();

        // gardamos si hemos de seguir al objeto y el objeto en si
        this.toFollow = toFollow;           // mi fan (a quien sigo)

        //Debug.LogError(audioSource.clip.name);
        Destroy(this.gameObject, audioEffect.length);

    }

    private void Update()
    {
        if (toFollow != null)
        {
            transform.position = toFollow.transform.position;
        }
    }


}
