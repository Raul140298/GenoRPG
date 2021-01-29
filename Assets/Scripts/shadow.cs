using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shadow : MonoBehaviour
{
    public GameObject target;
    public CharacterController control;
    public Vector3 startPosTarget, startPosObj, difference, auxFloor, auxScale, ScaleIni;
    public float floorShadow, scaleX, scaleY;
   
    void Start()
    {
        target = this.transform.parent.gameObject;
        startPosTarget = target.transform.position;
        startPosObj = this.transform.position;
        ScaleIni = this.transform.localScale;
        scaleX = this.transform.localScale.x;
        scaleY = this.transform.localScale.y;
    }
    void Update()
    {
        control = target.GetComponent<CharacterController>();

        if (control)//SI ES EL PERSONAJE
		{
            //actualizar la posición de la sombra
            floorShadow = control.floor;
            auxFloor = target.transform.position;
            auxFloor.y = floorShadow;
            difference = auxFloor - startPosTarget;
            this.transform.position = startPosObj + difference;

			//actualizar el tamaño de la sombra
			if (!control.canJump)
			{
                if (control.elevando == true)
                {//si se está elevando debemos encogerla
                    auxScale = this.transform.localScale;
                    auxScale.x -= scaleX/75 * Time.deltaTime;
                    auxScale.y -= scaleY/75 * Time.deltaTime;
                    this.transform.localScale = auxScale;
                }
                else
                {//si esta cayendo
                    auxScale = this.transform.localScale;
                    if (auxScale.x < scaleX && auxScale.y < scaleY)
                    {
                        auxScale.x += scaleX / 100 * Time.deltaTime;
                        auxScale.y += scaleY / 100 * Time.deltaTime;
                        this.transform.localScale = auxScale;
                    }
                }
            }
			else
			{
                this.transform.localScale = ScaleIni;
            }
        }
		else//SI NO ES EL PERSONAJE (solo tiene que actualizar la sombra por si se mueve)
		{
            //actualizar la posición de la sombra
            difference = target.transform.position - startPosTarget;
            this.transform.position = startPosObj + difference;
        }
    }
}
