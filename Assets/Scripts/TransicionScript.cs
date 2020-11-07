using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransicionScript : MonoBehaviour
{
	// Para controlar si empieza o no la transición
	public static bool start = false;
	// Para controlar si la transición es de entrada o salida
	public static bool isFadeIn = false;
	// Opacidad inicial del cuadrado de transición
	public static float alpha = 0;
	// Transición de 1 segundo
	public static float fadeTime = 1f;
	public static Camera camera1, camera2;

	public static void All_Cameras()
    {
        object[] Cams = GameObject.FindObjectsOfType(typeof(Camera));
        foreach (Camera C in Cams)
        {
            if (C.name == "Main Camera")
            {
                camera1 = C;
            }
            else if (C.name == "Battle Camera")
            {
                camera2 = C;
            }
        }
    }

    // Método para activar la transición de entrada
    public static void FadeIn()
    {
        start = true;
        isFadeIn = true;
    }

    // Método para activar la transición de salida
    public static void FadeOut()
    {
        isFadeIn = false;
    }

	// Dibujaremos un cuadrado con opacidad encima de la pantalla simulando una transición
	public static void OnGUI()
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
