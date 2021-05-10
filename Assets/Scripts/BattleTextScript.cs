using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleTextScript : MonoBehaviour
{
    public TextMeshPro textDisplay;
    public string[] sentences;
    public int index = 0;
    public float typingSpeed;
    public SpriteRenderer continueButton;
    //bool isActive = false, last = false;
    public SpriteRenderer cajaTextoSprite;
    public MeshRenderer textoSprite;
    CharacterController controller;
    public BattleSystem battleSystem;
    public GameObject posInicial;
    public GameObject bowsette;
    // start: Para controlar si empieza o no la transición
    // isFadeIn: Para controlar si la transición es de entrada o salida
    // alpha: Opacidad inicial del cuadrado de transición
    // fadeTime: Transición de 1 segundo
    bool start = false, isFadeIn = false;
    float alpha = 0, fadeTime = 1f;

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
        switch (battleSystem.state)
        {
            case BattleState.START:
                cajaTextoSprite.enabled = textoSprite.enabled = continueButton.enabled = false;
                break;
            case BattleState.PLAYERTURN:
                break;
            case BattleState.ENEMYTURN:
                break;
            case BattleState.NARRATIVE:
                cajaTextoSprite.enabled = textoSprite.enabled = true;
                break;
            case BattleState.LOST:
                break;
            case BattleState.WON:
                break;
            case BattleState.VACIO:
                break;
            case BattleState.RUN:
                break;
            default:
                print("Error de seleccion de estado\n");
                break;
        }
    }

    public IEnumerator Type()
    {
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void NextSentence()
    {
        continueButton.enabled = false;

        if (index < sentences.Length - 1)
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
		//isActive = false;
		//controller.state = State.ADVENTURE;
		if (controller.numNarrative == -1)
		{
			FadeIn();
			yield return new WaitForSeconds(fadeTime * 2);

			//controller.gameObject.transform.position = new Vector3(10.081f, 8.107f, 0f);


			FadeOut();
			//SoundSystemScript.PlaySoundtrack(controller.zonaActual.GetComponent<ZonaScript>().soundtrack.name);
			yield return new WaitForSeconds(fadeTime * 2);
		}
	}

	// Método para activar la transición de entrada
	public void FadeIn()
    {
        start = true;
        isFadeIn = true;
    }

    // Método para activar la transición de salida
    public void FadeOut()
    {
        isFadeIn = false;
    }

    // Dibujaremos un cuadrado con opacidad encima de la pantalla simulando una transición
    public void OnGUI()
    {
        // Si no empieza la transición salimos del evento directamente
        if (!start)
            return;

        // Si ha empezamos creamos un color con una opacidad inicial a 0
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);

        // Creamos una textura temporal para rellenar la pantalla
        Texture2D tex;
        tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.black);
        tex.Apply();

        // Dibujamos la textura sobre toda la pantalla
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex);

        // Controlamos la transparencia
        if (isFadeIn)
        {
            // Si es la de aparecer le sumamos opacidad
            alpha = Mathf.Lerp(alpha, 1.2f, fadeTime * 1.4f * Time.deltaTime);
        }
        else
        {
            // Si es la de desaparecer le restamos opacidad
            alpha = Mathf.Lerp(alpha, -0.75f, fadeTime * Time.deltaTime);
            // Si la opacidad llega a 0 desactivamos la transición
            if (alpha < 0) start = false;
        }

    }
}

