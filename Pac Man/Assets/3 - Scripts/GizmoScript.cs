using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoScript : MonoBehaviour {

    // cuando unity vaya a dibujar un guizmo o redibujarlo este codigo se ejecutara
    // solo se ejecutara cuando se tenga que actualiar el gizmo de este objeto
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        // dibujamos solo el wire
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);

        // para que dibuje un icono en vez del cubo (Este ha de estar guardado en la carpeta con nombre Gizmo)
        //Gizmos.DrawIcon(transform.position,/*nombre del icono en el proyecto*/);

    }

    // actua como la funcon anterior pero solo pintara el icono o el gizmo cuando este seleccionado
   private void OnDrawGizmosSelected()
    {

        // cambaimos el coor del gizmo
        Gizmos.color = Color.white;
        // dibujamos un gizmo de cubo en la posicion actual    y con su escala global
        Gizmos.DrawCube(transform.position, transform.lossyScale);
    }

}
