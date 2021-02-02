using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeScript : MonoBehaviour
{
    public CameraScript cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<CameraScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnBecameInvisible()
    {
        //print(unitNames + " se escondió\n");
        cam.removeUnitToObjetosEnCamara(this.gameObject);
    }

    // ... y habilítelo de nuevo cuando sea visible.
    void OnBecameVisible()
    {
        //print(unitNames + " es visible\n");
        cam.addUnitToObjetosEnCamara(this.gameObject);
    }
}
