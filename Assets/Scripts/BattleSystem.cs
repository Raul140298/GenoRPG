using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START,PLAYERTURN,ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    //OBJETOS O PERSONAJES QUE PELEAN
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject playerButtons;

    //POSICIONES EN LAS QUE SE INSTANCIAN
    //public Transform playerBattleStation;
    //public Transform enemyBattleStation;

    //DATOS DE LOS QUE PELEARAN
    public Unit playerUnit;
    public Unit enemyUnit;

    public BattleHUD playerHUD;
    //public BattleHUD enemyHUD;
    public BattleSystem battleByTurn;
	public GameObject playerBattleStation;
	public GameObject enemyBattleStation;
    public GameObject zonaBatalla;
    public BattleState state;

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
        //playerBattleStation.GetComponent<SpriteRenderer>().sprite = playerUnit.unitSprite;
        //enemyBattleStation.GetComponent<SpriteRenderer>().sprite = enemyUnit.unitSprite;

        All_Cameras();

        zonaBatalla = GameObject.Find("Zona Battalla");
        playerBattleStation = GameObject.Find("PlayerBattleStation");
        enemyBattleStation = GameObject.Find("EnemyBattleStation");

        this.playerUnit = playerPrefab.GetComponent<Unit>();
        this.enemyUnit = enemyPrefab.GetComponent<Unit>();

        playerBattleStation.GetComponent<SpriteRenderer>().sprite = this.playerUnit.unitSprite;
        enemyBattleStation.GetComponent<SpriteRenderer>().sprite = this.enemyUnit.unitSprite;
        enemyBattleStation.GetComponent<Animator>().runtimeAnimatorController = this.enemyUnit.unitBattleAnimator;

        destino = enemyBattleStation.transform.position + mov;

        state = BattleState.START;

        StartCoroutine(SetupBattle());
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
        if(state == BattleState.WON)
		{
            print("Ganaste\n");
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
    }

    IEnumerator playerTurn()
	{
        print("Turno del personaje\n"); 

        yield return new WaitForSeconds(1.5f);

        playerButtons.GetComponent<SpriteRenderer>().enabled = true;

        StartCoroutine(PlayerAttack());
    }

    public void OnAttackButton()
	{
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
        PlayerAttack();
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

    void All_Cameras()
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
}
