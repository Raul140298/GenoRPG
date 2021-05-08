using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    Transform target;
    CharacterController control;
    Vector3 prevPosition;
    Vector2 velocity;
    bool followPlayer = false;
    float posX, posY, smoothTime = 0.6f, posXF, posYF, smoothTimeF = 0.25f;
    public List<GameObject> objetosEnCamara = new List<GameObject>();

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z);
        prevPosition = transform.position;

        control = target.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
		posX = Mathf.SmoothDamp(
			transform.position.x,
			target.position.x,
			ref velocity.x,
			smoothTime,
			10f,
			Time.deltaTime);

		posY = Mathf.SmoothDamp(
			transform.position.y,
			control.floor,
			ref velocity.y,
			smoothTime,
			10f,
			Time.deltaTime);

        posXF = Mathf.SmoothDamp(
            transform.position.x,
            target.position.x,
            ref velocity.x,
            smoothTimeF,
            10f,
            Time.deltaTime);

        posYF = Mathf.SmoothDamp(
            transform.position.y,
            control.floor,
            ref velocity.y,
            smoothTimeF,
            10f,
            Time.deltaTime);

        if (!followPlayer && Math.Abs(control.floor-transform.position.y)>=0.2 ||
            Math.Abs(target.position.x - transform.position.x) >= 0.2)
		{
            followPlayer = true;
        }

        if (followPlayer)
        {
            //si no se deja de mover lo seguimos
            if (prevPosition.x != target.position.x || prevPosition.y != control.floor)
            {
                transform.position = new Vector3(
					 //target.position.x,
					 //control.floor,
					posX,
					posY,
					transform.position.z);
            }
            else //Si se ha detenido, se reinicia el cuadrado
            {
                followPlayer = false;
                transform.position = new Vector3(
                    //target.position.x,
                    //control.floor,
                    posXF,
                    posYF,
                    transform.position.z);
            }
        }

        prevPosition.x = target.position.x;
        prevPosition.y = control.floor;
        //prevPosition.x = posX;
        //prevPosition.y = posY;
        prevPosition.z = transform.position.z;

        actualizaTransformZ();

    }

    void actualizaTransformZ()
	{
        int count = objetosEnCamara.Count;

        if(count > 1)
		{
            objetosEnCamara.Sort(sortByY);
            for(int i=0; i< objetosEnCamara.Count; i++)
			{
                    objetosEnCamara[i].transform.position = new Vector3(
                    objetosEnCamara[i].transform.position.x,
                    objetosEnCamara[i].transform.position.y,
                    -i);     
            }
        }

    }

    public void addUnitToObjetosEnCamara(GameObject o)
	{
        objetosEnCamara.Add(o);
    }

    private static int sortByY(GameObject o1, GameObject o2)
    {
        if(o1.name == "player")
		{
            return o2.transform.position.y.CompareTo(o1.GetComponent<CharacterController>().floor);
        }
        else if (o2.name == "player")
		{
            return o2.GetComponent<CharacterController>().floor.CompareTo(o1.transform.position.y);
        }
		else
		{
            return o2.transform.position.y.CompareTo(o1.transform.position.y);
        }
    }

    public void removeUnitToObjetosEnCamara(GameObject o)
	{
        objetosEnCamara.Remove(o);
    }

}
