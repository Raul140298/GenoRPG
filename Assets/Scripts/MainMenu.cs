using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	GameObject mainMenu, creditosMenu;
	Camera Camera;

	void Start()
	{
		mainMenu = GameObject.Find("MainMenu");
		creditosMenu = GameObject.Find("CreditosMenu");
		creditosMenu.SetActive(false);
		Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
	}

	public void Jugar()
	{
		//playerController.state = State.ADVENTURE;
		mainMenu.SetActive(false);
	}

	public void Creditos()
	{
		mainMenu.SetActive(false);
		creditosMenu.SetActive(true);
	}

	public void Salir()
	{
		Application.Quit();
	}
}
