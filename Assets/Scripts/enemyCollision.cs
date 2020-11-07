using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class enemyCollision : MonoBehaviour
{
    public BattleSystem battleByTurn;

    // Detectamos la colisión con el personaje:
    void OnTriggerEnter2D(Collider2D col)
    {
        battleByTurn.playerPrefab = col.gameObject;
        battleByTurn.playerHUD.SetHUD(battleByTurn.playerPrefab.GetComponent<Unit>());

        battleByTurn.enemyPrefab = GameObject.FindGameObjectWithTag("Enemy");
        //battleByTurn.enemyHUD.SetHUD(battleByTurn.enemyPrefab.GetComponent<Unit>());

        SoundSystemScript.PlaySound("Sound_battle_start");
        SceneManager.LoadScene("BattleScene");
    } 
}
