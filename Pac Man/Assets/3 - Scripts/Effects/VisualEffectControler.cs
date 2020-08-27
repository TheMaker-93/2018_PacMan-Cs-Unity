using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffectControler : MonoBehaviour {

    [Header("Visual")]
    public SpriteRenderer sprRenderer;
    public Animator animator;
    public RuntimeAnimatorController animatorController;            // contenedor de las animacioens
    public string animationName;
    public float timeBerforeDestruction;

    public void setAnimatorController (RuntimeAnimatorController aController, string animationInControllerName)
    {
        animatorController = aController;           // guardamos el controler
        animationName = animationInControllerName;  // el nombre del animacion dentro de dicho controler
    }

	// Use this for initialization
	void Start () {
		
        if (animatorController != null && animationName != null)
        {
            animator.runtimeAnimatorController = animatorController;
            animator.Play(animationName);
        }
        
        Destroy(this.gameObject, timeBerforeDestruction    );

	}

}
