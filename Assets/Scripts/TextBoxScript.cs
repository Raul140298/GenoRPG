using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBoxScript : MonoBehaviour
{
    public TextMeshPro text;
    SpriteRenderer cajaTextoSprite;
    MeshRenderer textoSprite;
    CharController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<CharController>();
        cajaTextoSprite = this.GetComponent<SpriteRenderer>();
        textoSprite = this.transform.GetChild(0).GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (controller.state)
        {
            case State.ADVENTURE:
                cajaTextoSprite.enabled = textoSprite.enabled = false;
                break;
            case State.BATTLE:
                cajaTextoSprite.enabled = textoSprite.enabled = false;
                break;
            case State.NARRATIVE:
                cajaTextoSprite.enabled = textoSprite.enabled = true;
                break;
            default:
                print("Error de seleccion de estado\n");
                break;
        }
    }
}
