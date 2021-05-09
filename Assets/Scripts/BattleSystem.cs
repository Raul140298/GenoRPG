﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST, RUN, VACIO, NARRATIVE }
public enum Action { ITEM, SPECIAL, ATTACK, ETC, NONE }

public class BattleSystem : MonoBehaviour
{
    //OBJETOS O PERSONAJES QUE PELEAN
    public GameObject playerPrefab, enemyPrefab, playerButtons;
    //DATOS DE LOS QUE PELEARAN
    public Unit playerUnit, enemyUnit;
    public String enemyName;
    //DATOS DE IU
    public BattleHUD playerHUD;
    public GameObject playerBattleStation, enemyBattleStation, zonaBatalla, genoHUD;
    public CharacterController controller;
    public BattleState state;
    public Action action;
    public BattleTextScript battleText;
    //Animator enemyAnim;
    public Animator playerAnim, buttonAnim, enemyAnim;
    public Camera camera1, camera2;
    public bool firstAttack = false;

    Vector3 mov = new Vector3(-1, -1, 0), destino;
    bool enemyTurn = false;
    // start: Para controlar si empieza o no la transición
    // isFadeIn: Para controlar si la transición es de entrada o salida
    // alpha: Opacidad inicial del cuadrado de transición
    // fadeTime: Transición de 1 segundo
    bool start = false, isFadeIn = false;
    float alpha = 0, fadeTime = 1f;
    
    void Start()
    {
        zonaBatalla = GameObject.Find("Zona Battalla");
        buttonAnim = GameObject.Find("BattleButtons").GetComponent<Animator>();
    }

    public void Init()
    {
        //CUESTIONES PREVIAS:
        //Desactivo el movimiento del player al setearle el estado de BATTLE
        controller.state = State.BATTLE;

        //Posiciono al enemigo fuera de la pantalla, para que este luego se mueva a su posicion original
        enemyBattleStation.transform.position = new Vector3(3.5f, 2.82f, 0f);

        //Establezco la posición original del enemigo(la del enemyBattleStation)
        destino = enemyBattleStation.transform.position + mov;

        //Obtengo los datos del enemigo y del player
        this.playerUnit = playerPrefab.GetComponent<Unit>();
        this.enemyUnit = enemyPrefab.GetComponent<Unit>();

        playerBattleStation.GetComponent<SpriteRenderer>().sprite = this.playerUnit.unitSprite;
        playerBattleStation.transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = false;
        playerBattleStation.GetComponent<Animator>().runtimeAnimatorController = this.playerUnit.unitBattleAnimator;
        playerAnim = playerBattleStation.GetComponent<Animator>();
        playerHUD.SetHP(this.playerUnit.unitCurrHP);

        enemyBattleStation.GetComponent<SpriteRenderer>().sprite = this.enemyUnit.unitSprite;
        enemyBattleStation.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        enemyBattleStation.GetComponent<Animator>().runtimeAnimatorController = this.enemyUnit.unitBattleAnimator;
        enemyAnim = enemyBattleStation.GetComponent<Animator>();
        enemyAnim.SetBool("isDead", false);
        enemyName = this.enemyUnit.name;


        //Establezco el inicio de la batalla
        print("Comienza la Batalla\n");
        state = BattleState.START;
        action = Action.NONE;

        StartCoroutine(SetupBattle());
    }

    void Update()
    {
        if (enemyBattleStation.transform.position != destino && enemyTurn == false)
        {
            enemyBattleStation.transform.position = Vector3.MoveTowards(
                    enemyBattleStation.transform.position,
                    destino,
                    0.75f * Time.deltaTime);
        }
        else if (enemyTurn == true)
        {
            StartCoroutine(hideGenoHUD(false,0.05f));
            enemyBattleStation.transform.position = Vector3.MoveTowards(
                    enemyBattleStation.transform.position,
                    playerBattleStation.transform.position + new Vector3(0.1f, 0.1f, 0f),
                    0.75f * Time.deltaTime);
        }

        //Si es el turno del personajes
        if (buttonAnim.GetBool("PlayerTurn") == true)
        {
            StartCoroutine(hideGenoHUD(true,0.05f));
            if (Input.GetAxisRaw("Horizontal") > 0 && action != Action.ATTACK)
            {
                action = Action.ATTACK;
                SoundSystemScript.PlaySound("Sound_button");
                buttonAnim.SetFloat("eje X", 1f);
                buttonAnim.SetFloat("eje Y", 0f);
            }
            else if (Input.GetAxisRaw("Horizontal") < 0 && action != Action.SPECIAL)
            {
                action = Action.SPECIAL;
                SoundSystemScript.PlaySound("Sound_button");
                buttonAnim.SetFloat("eje X", -1f);
                buttonAnim.SetFloat("eje Y", 0f);
            }
            else if (Input.GetAxisRaw("Vertical") > 0 && action != Action.ITEM)
            {
                action = Action.ITEM;
                SoundSystemScript.PlaySound("Sound_button");
                buttonAnim.SetFloat("eje X", 0f);
                buttonAnim.SetFloat("eje Y", 1f);
            }
            else if (Input.GetAxisRaw("Vertical") < 0 && action != Action.ETC)
            {
                action = Action.ETC;
                SoundSystemScript.PlaySound("Sound_button");
                buttonAnim.SetFloat("eje X", 0f);
                buttonAnim.SetFloat("eje Y", -1f);
            }

            if (Input.GetKey("c"))
            {
                StartCoroutine(hideGenoHUD(false, 0.1f));
                SoundSystemScript.PlaySound("Sound_button");
                StartCoroutine(HideButtons());
                buttonAnim.SetBool("PlayerTurn", false);
                switch (action)
                {
                    case Action.ATTACK:
                        StartCoroutine(PlayerAttack());
                        break;
                    case Action.SPECIAL:
                        StartCoroutine(PlayerSpecial());
                        break;
                    case Action.ITEM:
                        StartCoroutine(PlayerItem());
                        break;
                    case Action.ETC:
                        state = BattleState.RUN;
                        StartCoroutine(EndBattle());
                        StartCoroutine(Run());
                        break;
                    case Action.NONE:
                        StartCoroutine(playerTurn());
                        break;
                    default:
                        print("Error de seleccion de accion\n");
                        break;
                }
                action = Action.NONE;
            }
        }

        if(state == BattleState.WON)
		{
            StartCoroutine(Won());
        }
    }

    IEnumerator HideButtons()
    {
        buttonAnim.SetFloat("eje X", 0f);
        buttonAnim.SetFloat("eje Y", 0f);
        yield return new WaitForSeconds(0.25f);
        playerButtons.GetComponent<SpriteRenderer>().enabled = false;
    }

    IEnumerator SetupBattle()
    {
        //obtiene info acerca de los UNIT
        playerHUD.SetHUD(playerUnit);

        //Tiempo de espera hasta que el enemigo llegue al centro
        yield return new WaitForSeconds(2.5f);

        //Tiempo de decisión
        yield return new WaitForSeconds(0.5f);
        if (playerUnit.unitSpeed > enemyUnit.unitSpeed)
        {
            state = BattleState.PLAYERTURN;
            StartCoroutine(playerTurn());
        }
        else
        {
            state = BattleState.ENEMYTURN;        
            if(firstAttack == false && enemyName == "enemy bowsette")
			{
                firstAttack = true;
                print("enemigo especial\n");
                StartCoroutine(EnemySpecial());
            }
			else
			{    
                StartCoroutine(EnemyTurn());
            }
        }
    }

    IEnumerator playerTurn()
    {
        print("Turno del personaje\n");

        //Me volteo y miro a cámara
        playerAnim.SetFloat("eje X", 1f);
        playerAnim.SetFloat("eje Y", 0f);

        yield return new WaitForSeconds(0.25f);

        //y después muestro los botones
        playerButtons.GetComponent<SpriteRenderer>().enabled = true;
        buttonAnim.SetFloat("eje X", 0f);
        buttonAnim.SetFloat("eje Y", 0f);
        buttonAnim.SetBool("PlayerTurn", true);
    }

    IEnumerator PlayerAttack()
    {
        //Tiempo para voltearse
        yield return new WaitForSeconds(0.25f);
        playerAnim.SetBool("isAttack", false);
        playerAnim.SetFloat("eje X", -1f);
        playerAnim.SetFloat("eje Y", 0f);

        //Tiempo del ataque
        yield return new WaitForSeconds(0.5f);

		//Animación completa del ataque---------------------------------------------------------
		playerAnim.SetFloat("eje X", 1f);
		playerAnim.SetFloat("eje Y", 0f);
        playerAnim.SetBool("isAttack", true);
        yield return new WaitForSeconds(0.35f);

        //Se debe hacer de esta manera para que las particulas puedan intercambiarase por las de cualquier ataque.
        print("Atacaste\n");
        GetCopyOfClass.GetCopyOf(playerBattleStation.transform.GetChild(0).GetComponent<ParticleSystem>(), playerUnit.unitAttackParticle.GetComponent<ParticleSystem>());
        var main = playerBattleStation.transform.GetChild(0).GetComponent<ParticleSystem>().main;
        main.duration = playerUnit.unitAttackParticle.GetComponent<ParticleSystem>().main.duration;
        playerBattleStation.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = playerUnit.unitAttackParticle.GetComponent<ParticleSystemRenderer>().sharedMaterial;
        playerBattleStation.transform.GetChild(0).GetComponent<ParticleSystem>().Play();

        SoundSystemScript.PlaySound("Sound_geno_fingershot");
        bool isDead = enemyUnit.TakeDamage(playerUnit.unitDamage);
        //Fin de la animacion

        yield return new WaitForSeconds(0.89f);
        //---------------------------------------------------------------------------------------
        playerAnim.SetFloat("eje X", -1f);
        playerAnim.SetFloat("eje Y", 0f);
        playerAnim.SetBool("isAttack", false);

        yield return new WaitForSeconds(0.25f);

        print("Enemigo recibio daño\n");
        //ANIMACION DE RECIBIR DAÑO Y SONIDO
        yield return new WaitForSeconds(1f);

        //yield return new WaitForSeconds(1f);
        //comprobar si esta muerto
        if (isDead)
        {
            enemyAnim.SetBool("isDead", true);
            yield return new WaitForSeconds(0.3f);
            SoundSystemScript.PlaySound("Sound_dead");
            yield return new WaitForSeconds(0.7f);
            state = BattleState.WON;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerSpecial()
    {
        if(this.playerUnit.unitCurrMP<=0)
		{
            StartCoroutine(playerTurn());
            yield break;
        }
        //Tiempo para voltearse
        yield return new WaitForSeconds(0.25f);
        playerAnim.SetBool("isAttack", false);
        playerAnim.SetFloat("eje X", -1f);
        playerAnim.SetFloat("eje Y", 0f);

        //Tiempo del ataque
        yield return new WaitForSeconds(0.5f);

        //Animación completa del ataque---------------------------------------------------------
        playerAnim.SetFloat("eje X", 0f);
        playerAnim.SetFloat("eje Y", -1f);
        playerAnim.SetBool("isAttack", true);
        yield return new WaitForSeconds(1f);

        //Se debe hacer de esta manera para que las particulas puedan intercambiarase por las de cualquier ataque.
        print("Especial\n");
        GetCopyOfClass.GetCopyOf(playerBattleStation.transform.GetChild(1).GetComponent<ParticleSystem>(), playerUnit.unitSpecialParticle.GetComponent<ParticleSystem>());
        var main = playerBattleStation.transform.GetChild(1).GetComponent<ParticleSystem>().main;
        main.duration = playerUnit.unitSpecialParticle.GetComponent<ParticleSystem>().main.duration;
        playerBattleStation.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().sharedMaterial = playerUnit.unitSpecialParticle.GetComponent<ParticleSystemRenderer>().sharedMaterial;
        playerBattleStation.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        SoundSystemScript.PlaySound("Sound_geno_laserbeam");
        bool haveMana = playerUnit.TakeMana(25);
        playerHUD.SetMP(playerUnit.unitCurrMP);
        bool isDead = enemyUnit.TakeDamage(playerUnit.unitDamage * 2);
        //Fin de la animacion

        yield return new WaitForSeconds(1f);
        //---------------------------------------------------------------------------------------
        playerAnim.SetFloat("eje X", -1f);
        playerAnim.SetFloat("eje Y", 0f);
        playerAnim.SetBool("isAttack", false);
        yield return new WaitForSeconds(0.25f);

        print("Enemigo recibio daño\n");
        //ANIMACION DE RECIBIR DAÑO Y SONIDO
        yield return new WaitForSeconds(1f);

        //yield return new WaitForSeconds(1f);
        //comprobar si esta muerto
        if (isDead)
        {
            enemyAnim.SetBool("isDead", true);
            yield return new WaitForSeconds(0.3f);
            SoundSystemScript.PlaySound("Sound_dead");
            yield return new WaitForSeconds(0.7f);
            state = BattleState.WON;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerItem()
    {
        //Tiempo para voltearse
        yield return new WaitForSeconds(0.25f);
        playerAnim.SetBool("isAttack", false);
        playerAnim.SetFloat("eje X", -1f);
        playerAnim.SetFloat("eje Y", 0f);

        print("Iteml\n");
        //Tiempo del ataque
        yield return new WaitForSeconds(0.5f);

        //Animación completa del ataque---------------------------------------------------------
        playerAnim.SetFloat("eje X", 0f);
        playerAnim.SetFloat("eje Y", 1f);
        playerAnim.SetBool("isAttack", true);
        yield return new WaitForSeconds(1.5f);

        //Se debe hacer de esta manera para que las particulas puedan intercambiarase por las de cualquier ataque.
        print("Restauraste Hp y Mp\n");
        SoundSystemScript.PlaySound("Sound_item");
        playerUnit.TakeDamage(-100);
        playerUnit.TakeMana(-25);
        playerHUD.SetHP(playerUnit.unitCurrHP);
        playerHUD.SetMP(playerUnit.unitCurrMP);
        //Fin de la animacion

        yield return new WaitForSeconds(0.5f);
        //---------------------------------------------------------------------------------------
        playerAnim.SetFloat("eje X", -1f);
        playerAnim.SetFloat("eje Y", 0f);
        playerAnim.SetBool("isAttack", false);
        yield return new WaitForSeconds(0.25f);

        //ANIMACION DE RECIBIR DAÑO Y SONIDO
        yield return new WaitForSeconds(1f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        print("Turno del enemigo\n");
        //Volteo al player en direccion al enemigo
        playerAnim.SetFloat("eje X", -1f);
        playerAnim.SetFloat("eje Y", 0f);

        //Activo la bandera para la animación de ataque y el tiempo se encarga de demorarse hasta que acabe la animacion
        enemyTurn = true;
        yield return new WaitForSeconds(1.5f);
        print("Enemigo ataca\n");
        SoundSystemScript.PlaySound("Sound_hit");
        bool isDead = playerUnit.TakeDamage(enemyUnit.unitDamage);
        playerHUD.SetHP(playerUnit.unitCurrHP);
        //Devuelvo al enemigo a su posicion en el mismo tiempo
        enemyTurn = false;
        yield return new WaitForSeconds(1.5f);

        //Evalúo si está muerto o no
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

    IEnumerator EnemySpecial()
    {
        StartCoroutine(hideGenoHUD(false, 0.02f));
        print("Turno del enemigo\n");
        //Volteo al player en direccion al enemigo
        playerAnim.SetFloat("eje X", -1f);
        playerAnim.SetFloat("eje Y", 0f);

        battleText.textDisplay.text = char.ToUpper(enemyBattleStation.transform.GetChild(1).name[0]) + enemyBattleStation.transform.GetChild(1).name.Substring(1);
        battleText.cajaTextoSprite.enabled = battleText.textoSprite.enabled = true;

        yield return new WaitForSeconds(1.5f);
        battleText.cajaTextoSprite.enabled = battleText.textoSprite.enabled = false;

        //Tiempo del ataque
        yield return new WaitForSeconds(1f);

        //Se debe hacer de esta manera para que las particulas puedan intercambiarase por las de cualquier ataque.
        print("Especial\n");
        GetCopyOfClass.GetCopyOf(enemyBattleStation.transform.GetChild(1).GetComponent<ParticleSystem>(), enemyUnit.unitSpecialParticle.GetComponent<ParticleSystem>());
        var main = enemyBattleStation.transform.GetChild(1).GetComponent<ParticleSystem>().main;
        main.duration = enemyUnit.unitSpecialParticle.GetComponent<ParticleSystem>().main.duration;
        enemyBattleStation.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().sharedMaterial = enemyUnit.unitSpecialParticle.GetComponent<ParticleSystemRenderer>().sharedMaterial;
        enemyBattleStation.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        SoundSystemScript.PlaySound("Sound_flame");
        yield return new WaitForSeconds(2f);
        StartCoroutine(EnemyAttackBurn());
        yield return new WaitForSeconds(1f);
        state = BattleState.PLAYERTURN;
        StartCoroutine(playerTurn());
    }

    IEnumerator EnemyAttackBurn()
	{
        while(true)
		{
            yield return new WaitForSeconds(1f);
            bool isDead = playerUnit.TakeDamage(5);
            playerHUD.SetHP(playerUnit.unitCurrHP);
            //Devuelvo al enemigo a su posicion en el mismo tiempo

            //Evalúo si está muerto o no
            if (isDead)
            {
                state = BattleState.LOST;
                StartCoroutine(EndBattle());
            }
        }
	}

    IEnumerator EndBattle()
    {
        //Devuelvo los botones a su posicion normal y los oculto (podría hacerlo en el Init() pero no funciona
        buttonAnim.SetFloat("eje X", 0f);
        buttonAnim.SetFloat("eje Y", 0f);
        playerButtons.GetComponent<SpriteRenderer>().enabled = false;

        switch (state)
        {
            case BattleState.LOST:
                state = BattleState.VACIO;
                print("Perdiste\n");
                StartCoroutine(Won());
                break;
            case BattleState.WON:
                print("Ganaste\n");
                yield return new WaitForSeconds(0.4f);
                SoundSystemScript.Stop();
                SoundSystemScript.PlaySound("Sound_win");
                //musica y animacion
                yield return new WaitForSeconds(1f);
                playerAnim.SetBool("won", true);
                print(enemyUnit.gameObject.name + " ha muerto \n");
                enemyUnit.Die();
                enemyBattleStation.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                break;
            case BattleState.RUN:
                print("Huiste con éxito\n");
                yield return new WaitForSeconds(4f);
                enemyPrefab.GetComponent<Unit>().enemyMove.enBatalla = false;
                break;
            default:
                break;
        }

        //tiempo de espera de la decision de huir o la animacion de muerte del enemigo
        yield return new WaitForSeconds(1.5f);
    }

    IEnumerator Won()
    {
        if (Input.GetKey("c") || state == BattleState.RUN || state == BattleState.VACIO)
        {
            FadeIn();
            yield return new WaitForSeconds(fadeTime);
            SoundSystemScript.Stop();
            SoundSystemScript.PlaySoundtrack(playerPrefab.GetComponent<CharacterController>().zonaActual.GetComponent<ZonaScript>().soundtrack.name);
            camera2.enabled = false;
            camera1.enabled = true;
            FadeOut();
            yield return new WaitForSeconds(fadeTime);

            //Desactivo el BattleSystem y activo el controller
            this.enabled = false;
            GameObject.Find("player").GetComponent<CharacterController>().state = State.ADVENTURE;
            GameObject.Find("player").GetComponent<CharacterController>().canBattle = true;

            //Retornamos la animacion al por defecto
            playerAnim.SetBool("won", false);
            playerAnim.SetFloat("eje X", -1f);
            playerAnim.SetFloat("eje Y", 0f);

            if(enemyName == "enemy bowsette" || state == BattleState.VACIO)
			{
                yield return new WaitForSeconds(1f);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            }
            genoHUD.SetActive(true);
        }
    }
    
    IEnumerator Run()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(Won());
    }

    IEnumerator hideGenoHUD(bool b, float x)
	{
        yield return new WaitForSeconds(x);
        genoHUD.SetActive(b);
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
