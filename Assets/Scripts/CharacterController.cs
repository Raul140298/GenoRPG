using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum State { ADVENTURE, BATTLE, TELEPORT, NARRATIVE }

public class CharacterController : MonoBehaviour
{
	#region variables
	//PUBLICAS
	public float floor;
	public float oposY;
	public bool canJump = true, elevando = false, choca = false, canBattle = true;
	public BattleSystem battleByTurn;
	public GameObject zonaBatalla, zonaActual;
	public State state;
	public Camera camera1, camera2;
	public Rigidbody2D body;
	public NarrativeTextScript narrative;
	//PRIVADAS
	float ejeY, topeY, anteriorY, chocaX, chocaY, speedJump = 4f, gravity = 1f;
	Vector3 salto = new Vector3(0f, 1f, 0f), mov;
	Animator anim;
	Unit player;
	// start: Para controlar si empieza o no la transición
	// isFadeIn: Para controlar si la transición es de entrada o salida
	// alpha: Opacidad inicial del cuadrado de transición
	// fadeTime: Transición de 1 segundo
	bool start = false, isFadeIn = false;
	float alpha = 0, fadeTime = 1f;
	#endregion

	// Start is called once
	void Start()
	{
		battleByTurn.controller = this;
		player = GetComponent<Unit>();
		anim = GetComponent<Animator>();
		body = GetComponent<Rigidbody2D>();
		zonaBatalla = GameObject.Find("Zona Battalla");
		zonaActual = GameObject.Find("Zona Kero Sewers");
		narrative = GameObject.Find("TextBox").GetComponent<NarrativeTextScript>();
		floor = transform.position.y;
		SoundSystemScript.PlaySoundtrack(zonaActual.GetComponent<ZonaScript>().soundtrack.name);
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

	void All_Cameras()//Ordena las cámaras
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
		//Obtenemos el movimiento
		mov = new Vector3(
			2 * Input.GetAxisRaw("Horizontal"),
			Input.GetAxisRaw("Vertical"),
			0
		);

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
			if(elevando)GetComponent<CapsuleCollider2D>().isTrigger = true;

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
		oposY = other.transform.position.y;


		if (other.gameObject.name.Contains("enemy") && canBattle == true 
			&& (Mathf.Abs(oposY - floor) <= 0.075f))
		{
			canBattle = false;
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
		else if (other.gameObject.name.Contains("teleport"))
		{

			state = State.TELEPORT;
			//SoundSystemScript.PlaySound("Sound_battle_start");
			print("Te teletransportaste\n");
			FadeIn();

			yield return new WaitForSeconds(fadeTime*2);

			this.gameObject.transform.position = other.GetComponent<WarpScript>().target.transform.GetChild(0).transform.position;

			//Chanco los datos de la zona de batalla de acuerdo a la zona actual.
			zonaBatalla.GetComponent<SpriteRenderer>().sprite = zonaActual.GetComponent<ZonaScript>().battleSprite;
			//Si es una nueva zona, debemos poder su música
			//SoundSystemScript.PlaySoundtrack(zonaActual.GetComponent<ZonaScript>().battleSoundtrack.name);		
			FadeOut();
			state = State.ADVENTURE;
			yield return new WaitForSeconds(fadeTime*2);
			

		}
		else if (other.gameObject.name.Contains("PipeIni"))
		{

			state = State.TELEPORT;
			SoundSystemScript.PlaySound("Sound_pipe");
			print("Te metiste al tubo\n");
			FadeIn();

			yield return new WaitForSeconds(fadeTime*3);

			this.gameObject.transform.position = other.transform.parent.GetComponent<PipeScript>().target.transform.position;

			//Chanco los datos de la zona de batalla de acuerdo a la zona actual.
			//zonaBatalla.GetComponent<SpriteRenderer>().sprite = zonaActual.GetComponent<ZonaScript>().battleSprite;
			//Si es una nueva zona, debemos poder su música
			//SoundSystemScript.PlaySoundtrack(zonaActual.GetComponent<ZonaScript>().battleSoundtrack.name);		
			FadeOut();
			state = State.ADVENTURE;
			SoundSystemScript.PlaySound("Sound_pipe");
			yield return new WaitForSeconds(fadeTime*3);
			

		}
		else if (other.gameObject.name.Contains("narrative"))
		{
			other.GetComponent<PolygonCollider2D>().enabled = false;
			narrative.sentences = other.GetComponent<NarrativeLauncherScript>().sentences;
			state = State.NARRATIVE;	
		}
		else if (other.gameObject.name.Contains("wall") && elevando == false)
		{
			choca = true;
			chocaX = mov.x;
			chocaY = mov.y;
		}
		else if (other.gameObject.name.Contains("Zona"))
		{
			zonaActual = other.gameObject;
			//SoundSystemScript.PlaySoundtrack(zonaActual.GetComponent<ZonaScript>().soundtrack.name);
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
