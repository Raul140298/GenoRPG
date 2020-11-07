using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START,PLAYERTURN,ENEMYTURN, WON, LOST }
public enum Action { ITEM, SPECIAL, ATTACK, ETC }

public class BattleSystem : MonoBehaviour
{
    //OBJETOS O PERSONAJES QUE PELEAN
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject playerButtons;

    //DATOS DE LOS QUE PELEARAN
    public Unit playerUnit;
    public Unit enemyUnit;

    //DATOS DE IU
    public BattleHUD playerHUD;
	public GameObject playerBattleStation;
	public GameObject enemyBattleStation;
    public GameObject zonaBatalla;
    public CharacterController controller;
    public BattleState state;
    public Action action;

    //Animator enemyAnim;
    public Animator playerAnim;
    public Animator buttonAnim;

    Vector3 mov = new Vector3(-1,-1,0);
    Vector3 destino;

    // Para controlar si empieza o no la transición
    bool start = false;
    // Para controlar si la transición es de entrada o salida
    bool isFadeIn = false;
    // Opacidad inicial del cuadrado de transición
    float alpha = 0;
    // Transición de 1 segundo
    float fadeTime = 1f;
    public Camera camera1, camera2;

    void Start()
    {
        zonaBatalla = GameObject.Find("Zona Battalla");
        buttonAnim = GameObject.Find("BattleButtons").GetComponent<Animator>();
    }

    void Update()
	{
        if(enemyBattleStation.transform.position != destino)
		{
            enemyBattleStation.transform.position = Vector3.MoveTowards(
                    enemyBattleStation.transform.position,
                    destino,
                    0.75f * Time.deltaTime);
        }

        //Si se está moviendo
        if (buttonAnim.GetBool("PlayerTurn") == true)
        {
            if(Input.GetAxisRaw("Horizontal") > 0)
			{
                SoundSystemScript.PlaySound("Sound_button");
                buttonAnim.SetBool("PlayerTurn", false);
                buttonAnim.SetFloat("eje X", 1f);
                buttonAnim.SetFloat("eje Y", 0f);
                action = Action.ATTACK;
                StartCoroutine(PlayerAttack());
            }
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                SoundSystemScript.PlaySound("Sound_button");
                buttonAnim.SetFloat("eje X", -1f);
                buttonAnim.SetFloat("eje Y", 0f);
                action = Action.SPECIAL;
            }
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                SoundSystemScript.PlaySound("Sound_button");
                buttonAnim.SetFloat("eje X", 0f);
                buttonAnim.SetFloat("eje Y", 1f);
                action = Action.ITEM;
            }
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                SoundSystemScript.PlaySound("Sound_button");
                buttonAnim.SetFloat("eje X", 0f);
                buttonAnim.SetFloat("eje Y", -1f);
                action = Action.ETC;
            }
        }
    }

    public void Init()
	{
        print("Comienza la Batalla\n");

        controller.state = State.BATTLE;

        enemyBattleStation.transform.position = new Vector3(3.5f, 2.82f, 0f);

        this.playerUnit = playerPrefab.GetComponent<Unit>();
        this.enemyUnit = enemyPrefab.GetComponent<Unit>();
        
        playerBattleStation.GetComponent<SpriteRenderer>().sprite = this.playerUnit.unitSprite;
        playerBattleStation.GetComponent<Animator>().runtimeAnimatorController = this.playerUnit.unitBattleAnimator;
        playerAnim = playerBattleStation.GetComponent<Animator>();

        enemyBattleStation.GetComponent<SpriteRenderer>().sprite = this.enemyUnit.unitSprite;
        enemyBattleStation.GetComponent<Animator>().runtimeAnimatorController = this.enemyUnit.unitBattleAnimator;

        destino = enemyBattleStation.transform.position + mov;

        state = BattleState.START;
    
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
	{   //obtiene info acerca de los UNIT
        //yield return new WaitForSeconds(2f);
        playerHUD.SetHUD(playerUnit);
        //enemyHUD.SetHUD(enemyUnit);

        //enemyBattleStation.transform.position = Vector3.MoveTowards(
        //				enemyBattleStation.transform.position,
        //              enemyBattleStation.transform.position + mov,
        //				0.5f * Time.deltaTime);

        //acá tengo que hacer que de alguna forma no se teletransporte, sino en cambio se mueva natural.
        //enemyBattleStation.transform.position = enemyBattleStation.transform.position + mov;

        yield return new WaitForSeconds(3f);

        if (playerUnit.unitSpeed > enemyUnit.unitSpeed)
		{
            state = BattleState.PLAYERTURN;
            StartCoroutine(playerTurn());
        }
        else
		{
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());

        }
        
    }

    IEnumerator PlayerAttack()
	{
        print("Atacaste\n");
        playerAnim.SetFloat("eje X", -1f);
        SoundSystemScript.PlaySound("Sound_button");
        yield return new WaitForSeconds(1.5f);
        //Atacar al enemigo:
        bool isDead = enemyUnit.TakeDamage(playerUnit.unitDamage);

        //enemyHUD.SetHP(enemyUnit.unitCurrHP);

        //yield return new WaitForSeconds(2f);

		//comprobar si esta muerto
		if (isDead)
		{
            state = BattleState.WON;
            StartCoroutine(EndBattle());
		} else
		{
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        //cambiar el estado
	}

    IEnumerator EnemyTurn()
	{
        print("Turno del enemigo\n");
        playerAnim.SetFloat("eje X", -1f);
        playerButtons.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(1.5f);
        
        bool isDead = playerUnit.TakeDamage(enemyUnit.unitDamage);
        playerHUD.SetHP(playerUnit.unitCurrHP);

        print("Enemigo ataca\n");
   
        if (isDead)
        {
            state = BattleState.LOST;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.PLAYERTURN;
            StartCoroutine(playerTurn());
        }
    }

    IEnumerator EndBattle()
	{
        buttonAnim.SetFloat("eje X", 0f);
        buttonAnim.SetFloat("eje Y", 0f);
        playerButtons.GetComponent<SpriteRenderer>().enabled = false;

        if (state == BattleState.WON)
		{
            print("Ganaste\n");
            print(enemyUnit.gameObject.name + " ha muerto \n");
            enemyUnit.Die();
        }
		else
		{
            print("Perdiste\n");
        }

        yield return new WaitForSeconds(1.5f);

        FadeIn();

        //player.GetComponent<CharacterController>().enabled = false;

        SoundSystemScript.Stop();

        yield return new WaitForSeconds(fadeTime);

        SoundSystemScript.PlaySoundtrack(playerPrefab.GetComponent<CharacterController>().zonaActual.GetComponent<ZonaScript>().soundtrack.name);

        camera2.enabled = false;
        camera1.enabled = true;
        
        FadeOut();

        yield return new WaitForSeconds(fadeTime);

        this.enabled = false;
        GameObject.Find("player").GetComponent<CharacterController>().state = State.ADVENTURE;
    }

    IEnumerator playerTurn()
	{
        print("Turno del personaje\n");

        playerAnim.SetFloat("eje X", 1f);
        playerAnim.SetFloat("eje Y", 0f);

        yield return new WaitForSeconds(1.5f);

        playerButtons.GetComponent<SpriteRenderer>().enabled = true;

        buttonAnim.SetFloat("eje X", 0f);
        buttonAnim.SetFloat("eje Y", 0f);
        buttonAnim.SetBool("PlayerTurn", true);
    }

    public void OnAttackButton()
	{
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
        PlayerAttack();
    }

	#region Transicion
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

	#endregion

}
