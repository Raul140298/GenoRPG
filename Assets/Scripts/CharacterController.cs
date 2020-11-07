using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum State { ADVENTURE, BATTLE }

public class CharacterController : MonoBehaviour
{
	#region variables
	public float floor;
	float ejeY;
	float topeY;
	float anteriorY;
	float chocaX;
	float chocaY;
	public bool canJump = true;
	public bool elevando = false;
	public bool choca = false;
	float speedJump = 4f;
	float gravity = 1f;
	Vector3 salto = new Vector3(0f, 1f, 0f);
	Vector3 mov;
	Animator anim;
	Unit player;
	public BattleSystem battleByTurn;
	public GameObject zonaBatalla;
	public GameObject zonaActual;
	Rigidbody2D body;
	public State state;
	// Para controlar si empieza o no la transición
	bool start = false;
	// Para controlar si la transición es de entrada o salida
	bool isFadeIn = false;
	// Opacidad inicial del cuadrado de transición
	float alpha = 0;
	// Transición de 1 segundo
	float fadeTime = 1f;
	public Camera camera1, camera2;
	#endregion

	// Start is called once
	void Start()
	{
		player = GetComponent<Unit>();
		anim = GetComponent<Animator>();
		body = GetComponent<Rigidbody2D>();
		zonaBatalla = GameObject.Find("Zona Battalla");
		zonaActual = GameObject.Find("Zona Kero Sewers");
		battleByTurn.controller = this;

		floor = transform.position.y;

		SoundSystemScript.PlaySoundtrack("Soundtrack_forest_maze");

		All_Cameras();

		state = State.ADVENTURE;
	}
	// Update is called once per frame
	void Update()
	{   	
		if (state == State.ADVENTURE)
		{
			ManageMovement();
			ManageJump();
		}
	}

	void FixedUpdate()
	{
		//Si se está moviendo
		if (mov != Vector3.zero)
		{
			anim.SetFloat("movX", mov.x);
			anim.SetFloat("movY", mov.y);
			anim.SetBool("walking", true);
		}
		else
		{
			anim.SetBool("walking", false);
		}
	}

	void All_Cameras()
	{
		object[] Cams = GameObject.FindObjectsOfType(typeof(Camera));
		foreach (Camera C in Cams)
		{
			if(C.name == "Main Camera")
			{
				camera1 = C;
			}
			else if (C.name == "Battle Camera")
			{
				camera2 = C;
			}
		}
	}

	void ManageMovement()
	{
		//obtenemos el movimiento
		mov = new Vector3(
			2 * Input.GetAxisRaw("Horizontal"),
			Input.GetAxisRaw("Vertical"),
			0
		);

		//if (speed < maxSpeed)
		//{
		//	speed += acceleration * Time.deltaTime;
		//}

		if (canJump)//si está en el suelo
		{
			GetComponent<CapsuleCollider2D>().isTrigger = false;
			if (choca == false)
			{
				transform.position = Vector3.MoveTowards(
						transform.position,
						transform.position + mov,
						player.unitSpeed * Time.deltaTime
						//speed * player.unitSpeed * Time.deltaTime
						);
			}
			else
			{   //Solo saldrá del bloque si es que se ha dejado de presionar los botones anteriores
				if (!comparaSignos(chocaX, mov.x) || !comparaSignos(chocaY, mov.y))
				{
					choca = false;
				}
			}
		}
		else //si ya saltó (elevando o cayendo)
		{   //basicamente para disminuir la velocidad en el aire
			GetComponent<CapsuleCollider2D>().isTrigger = true;
			anteriorY = gameObject.transform.position.y;
			transform.position = Vector3.MoveTowards(
						transform.position,
						transform.position + mov,
						0.75f * Time.deltaTime
						);
			//La diferencia de posicion (distancia) en ejeY actualiza el piso y el tope
			floor += transform.position.y - anteriorY;
			topeY += transform.position.y - anteriorY;
		}
	}

	void ManageJump()
	{
		ejeY = gameObject.transform.position.y;

		if (canJump)
		{
			floor = gameObject.transform.position.y;
			topeY = floor + 0.25f;
			
			if (Input.GetKey("c") && ejeY < topeY)
			{
				floor = ejeY;
				canJump = false;
				elevando = true;
				SoundSystemScript.PlaySound("Sound_jump");
			}
		}
		else
		{
			if (elevando == true)
			{
				transform.position = Vector3.MoveTowards(
						transform.position,
						transform.position + salto,
						speedJump * Time.deltaTime
						);
				if (transform.position.y >= topeY)
				{//si se ha superado al tope, lo devolvemos
					elevando = false;
					Vector3 newPosition = new Vector3(transform.position.x, topeY, transform.position.z);
					transform.position = newPosition;
				}
			}
			else
			{
				transform.position = Vector3.MoveTowards(
						transform.position,
						transform.position - salto,
						gravity * Time.deltaTime
						);
				if (transform.position.y <= floor)
				{//si ha traspasado el piso lo devolvemos
					canJump = true;//ya se ha llegado al piso
					Vector3 newPosition = new Vector3(transform.position.x, floor, transform.position.z);
					transform.position = newPosition;
				}
			}
		}
	}

	IEnumerator OnTriggerEnter2D(Collider2D other)
	{
		if (!other.gameObject.name.Contains("wall"))
		{
			SoundSystemScript.PlaySound("Sound_battle_start");
			print("Chocaste con un enemigo\n");
			FadeIn();
			
			//player.GetComponent<CharacterController>().enabled = false;
			yield return new WaitForSeconds(fadeTime);

			//Chanco los datos de la zona de batalla de acuerdo a la zona actual.
			zonaBatalla.GetComponent<SpriteRenderer>().sprite = zonaActual.GetComponent<ZonaScript>().battleSprite;
			SoundSystemScript.PlaySoundtrack(zonaActual.GetComponent<ZonaScript>().battleSoundtrack.name);

			camera1.enabled = false;
			camera2.enabled = true;

			battleByTurn.playerPrefab = gameObject;
			battleByTurn.enemyPrefab = other.gameObject;

			battleByTurn.enabled = true;
			battleByTurn.Init();
			
			FadeOut();

			yield return new WaitForSeconds(fadeTime);
		}
		else
		{
			choca = true;
			chocaX = mov.x;
			chocaY = mov.y;
		}
	}

	bool comparaSignos(float x, float y)
	{
		if (x < 0 && y < 0)
		{
			return true;
		}
		else if (x > 0 && y > 0)
		{
			return true;
		}
		else return false;
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
