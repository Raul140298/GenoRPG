using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WarpScript : MonoBehaviour
{
    // Para almacenar el punto de destino
    public GameObject target;
    // Para almacenar el mapa de destino
    public GameObject targetMap;


    void Awake()
    {
        // Nos aseguraremos de que target se ha establecido o lanzaremos except
        Assert.IsNotNull(target);

        // Si queremos podemos esconder el debug de los Warps
        GetComponent<SpriteRenderer>().enabled = false;
        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;

        Assert.IsNotNull(targetMap);
    }
}
