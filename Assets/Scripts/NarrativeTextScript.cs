using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NarrativeTextScript : MonoBehaviour
{
    public TextMeshPro textDisplay;
    public string[] sentences;
    public int index = 0;
    public float typingSpeed;
    public SpriteRenderer continueButton;
    bool isActive = false;
    SpriteRenderer cajaTextoSprite;  
    MeshRenderer textoSprite;
    CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        cajaTextoSprite = this.GetComponent<SpriteRenderer>();
        textoSprite = this.transform.GetChild(0).GetComponent<MeshRenderer>();
        continueButton = this.transform.GetChild(1).GetComponent<SpriteRenderer>();
        textDisplay.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        switch (controller.state)
        {
            case State.ADVENTURE:
                cajaTextoSprite.enabled = textoSprite.enabled = continueButton.enabled = false;               
                break;
            case State.BATTLE:
                break;
            case State.NARRATIVE:
                cajaTextoSprite.enabled = textoSprite.enabled = true;
                if (index < sentences.Length)
                {
                    if (isActive == false)
                    {
                        isActive = true;
                        StartCoroutine(Type());
                    }
                    if (textDisplay.text == sentences[index])
                    {
                        continueButton.enabled = true;
                        if (Input.GetKey("c"))
                        {
                            NextSentence();
                        }
                    }   
                }
                break;
            case State.TELEPORT:
                break;
            default:
                print("Error de seleccion de estado\n");
                break;
        }
    }

    IEnumerator Type()
	{
        foreach(char letter in sentences[index].ToCharArray())
		{
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
	}

    public void NextSentence()
	{
        continueButton.enabled = false;

        if(index < sentences.Length - 1)
		{
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
		}
		else
		{
            textDisplay.text = "";
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
	{
        yield return new WaitForSeconds(0.5f);
        index = 0;
        isActive = false;
        controller.state = State.ADVENTURE;
    }
}
