﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST, RUN }
public enum Action { ITEM, SPECIAL, ATTACK, ETC, NONE }

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

    Vector3 mov = new Vector3(-1, -1, 0);
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

    bool enemyTurn = false;

    void Start()
    {
        zonaBatalla = GameObject.Find("Zona Battalla");
        buttonAnim = GameObject.Find("BattleButtons").GetComponent<Animator>();
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
            enemyBattleStation.transform.position = Vector3.MoveTowards(
                    enemyBattleStation.transform.position,
                    playerBattleStation.transform.position + new Vector3(0.1f, 0.1f, 0f),
                    0.75f * Time.deltaTime);
        }

        //Si es el turno del personaje
        if (buttonAnim.GetBool("PlayerTurn") == true)
        {
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
                SoundSystemScript.PlaySound("Sound_button");
                StartCoroutine(HideButtons());
                switch (action)
                {
                    case Action.ATTACK:
                        StartCoroutine(PlayerAttack());
                        break;
                    case Action.SPECIAL:
                        StartCoroutine(PlayerAttack());
                        break;
                    case Action.ITEM:
                        StartCoroutine(PlayerAttack());
                        break;
                    case Action.ETC:
                        state = BattleState.RUN;
                        StartCoroutine(EndBattle());
                        break;
                    default:
                        print("Error de seleccion de accion\n");
                        break;
                }

                buttonAnim.SetBool("PlayerTurn", false);
                action = Action.NONE;
            }
        }
    }

    public void Init()
    {
        //CUESTIONES PREVIAS:
        //Desactivo el movimiento del player al setearle el estado de BATTLE
        controller.state = State.BATTLE;

        //Posiciono al enemigo fuera de la pantalla, para que este luego se mueva a su posicion original
        enemyBattleStation.transform.position = new Vector3(3.5f, 2.82f, 0f);

        //Obtengo los datos del enemigo y del player
        this.playerUnit = playerPrefab.GetComponent<Unit>();
        this.enemyUnit = enemyPrefab.GetComponent<Unit>();

        playerBattleStation.GetComponent<SpriteRenderer>().sprite = this.playerUnit.unitSprite;
        playerBattleStation.GetComponent<Animator>().runtimeAnimatorController = this.playerUnit.unitBattleAnimator;
        playerAnim = playerBattleStation.GetComponent<Animator>();

        enemyBattleStation.GetComponent<SpriteRenderer>().sprite = this.enemyUnit.unitSprite;
        enemyBattleStation.GetComponent<Animator>().runtimeAnimatorController = this.enemyUnit.unitBattleAnimator;

        //Establezco la posición original del enemigo(la del enemyBattleStation)
        destino = enemyBattleStation.transform.position + mov;

        //Establezco el inicio de la batalla
        print("Comienza la Batalla\n");
        state = BattleState.START;
        action = Action.NONE;

        StartCoroutine(SetupBattle());
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
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerAttack()
    {
        //Tiempo para voltearse
        yield return new WaitForSeconds(0.25f);
        playerAnim.SetFloat("eje X", -1f);

        //Tiempo del ataque
        yield return new WaitForSeconds(2f);
        print("Atacaste\n");
        bool isDead = enemyUnit.TakeDamage(playerUnit.unitDamage);
        yield return new WaitForSeconds(1f);

        //comprobar si esta muerto
        if (isDead)
        {
            state = BattleState.WON;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        print("Turno del enemigo\n");
        //Volteo al player en direccion al enemigo
        playerAnim.SetFloat("eje X", -1f);

        //Activo la bandera para la animación de ataque y el tiempo se encarga de demorarse hasta que acabe la animacion
        enemyTurn = true;
        yield return new WaitForSeconds(1.5f);
        print("Enemigo ataca\n");
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

    IEnumerator EndBattle()
    {
        //Devuelvo los botones a su posicion normal y los oculto (podría hacerlo en el Init() pero no funciona
        buttonAnim.SetFloat("eje X", 0f);
        buttonAnim.SetFloat("eje Y", 0f);
        playerButtons.GetComponent<SpriteRenderer>().enabled = false;

        switch (state)
        {
            case BattleState.LOST:
                print("Perdiste\n");
                break;
            case BattleState.WON:
                print("Ganaste\n");
                print(enemyUnit.gameObject.name + " ha muerto \n");
                enemyUnit.Die();
                break;
            case BattleState.RUN:
                print("Huiste con éxito\n");
                break;
            default:
                break;
        }

        //tiempo de espera de la decision de huir o la animacion de muerte del enemigo
        yield return new WaitForSeconds(1f);
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
