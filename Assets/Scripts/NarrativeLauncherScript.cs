using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeLauncherScript : MonoBehaviour
{
    public string[] sentences;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
