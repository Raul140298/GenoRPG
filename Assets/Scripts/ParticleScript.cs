using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    public ParticleSystem emitter;

    // Start is called before the first frame update
    void Start()
    {
        //Emitter.startLifetime = Vector3.Distance(Emitter.transform.position, target.position) / Emitter.startSpeed;
        emitter = this.GetComponent<ParticleSystem>();
        emitter.Stop();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
