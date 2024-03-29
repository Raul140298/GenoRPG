using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	GameObject mainMenu, creditosMenu;
	Camera Camera;
	public TextMeshPro[] flechasDeBotones = new TextMeshPro[3];
	public int botonSeleccionado;
	private float pressedSpaceTime;
	public Animator transitionAnim;

	void Start()
	{
		mainMenu = this.gameObject;
		//creditosMenu = GameObject.Find("CreditosMenu");
		//creditosMenu.SetActive(false);
		Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
		flechasDeBotones[0] = GameObject.Find("Start").transform.GetChild(0).GetComponent<TextMeshPro>();
		flechasDeBotones[1] = GameObject.Find("Options").transform.GetChild(0).GetComponent<TextMeshPro>();
		flechasDeBotones[2] = GameObject.Find("Quit").transform.GetChild(0).GetComponent<TextMeshPro>();
		for(int i=0; i <= 2; i++)
		{
			flechasDeBotones[i].GetComponent<MeshRenderer>().enabled = false;
		}
		botonSeleccionado = 0;

		actualizaBoton();
		//StartCoroutine(ManageOptions());
	}

	private void Update()
	{
		if (Input.GetKey("down") && Time.time - pressedSpaceTime > 0.2f)
		{
			botonSeleccionado++;
			if (botonSeleccionado == 3) botonSeleccionado = 0;
			pressedSpaceTime = Time.time;
			actualizaBoton();
			SoundSystemScript.PlaySound("Sound_button");
		}
		if (Input.GetKey("up") && Time.time - pressedSpaceTime > 0.2f)
		{
			botonSeleccionado--;
			if (botonSeleccionado == -1) botonSeleccionado = 2;
			pressedSpaceTime = Time.time;
			actualizaBoton();
			SoundSystemScript.PlaySound("Sound_button");
		}

		if (Input.GetKey("c") || Input.GetKey("space") || Input.GetKey(KeyCode.Return))
		{
			switch (botonSeleccionado)
			{
				case 0:
					Jugar();
					break;
				case 1:
					Opciones();
					break;
				case 2:
					Salir();
					break;
				default:
					print("Error de seleccion de boton\n");
					break;
			}
		}
	}

	public void actualizaBoton()
	{
		for (int i = 0; i <= 2; i++)
		{
			if (i == botonSeleccionado) flechasDeBotones[i].GetComponent<MeshRenderer>().enabled = true;
			else if (flechasDeBotones[i].enabled == true) flechasDeBotones[i].GetComponent<MeshRenderer>().enabled = false;
		}
	}

	public void Jugar()
	{
		StartCoroutine(loadScene());
	}

	public void Opciones()
	{
	}

	public void Salir()
	{
		Application.Quit();
	}

	IEnumerator loadScene()
	{
		SoundSystemScript.Stop();
		this.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
		this.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
		this.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
		this.transform.GetChild(1).transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
		this.transform.GetChild(2).GetComponent<MeshRenderer>().enabled = false;
		this.transform.GetChild(2).transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
		this.transform.GetChild(3).GetComponent<MeshRenderer>().enabled = false;
		this.transform.GetChild(3).transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
		transitionAnim.SetTrigger("end");
		yield return new WaitForSeconds(9.167f);
		transitionAnim.SetTrigger("finish");
		yield return new WaitForSeconds(0.831f);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
}
