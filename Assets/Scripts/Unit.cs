using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;

public enum SceneState { ADVENTURESCENE, BATTLESCENE}

public class Unit : MonoBehaviour
{
    public ScriptableCharacters character;
    public string unitNames;
    public int unitLvl;
    public int unitDamage;
    public int unitMaxHP;
    public int unitCurrHP;
    public float unitSpeed;
    public Sprite unitSprite;
    CapsuleCollider2D coll;
    Vector3 auxiliar;
    public GameObject sombraPrefab;
    public UnityEditor.Animations.AnimatorController unitBattleAnimator;

    void Start()
    {   //extrae la info de los scriptables characters
        unitNames = character.names;
        unitLvl = character.lvl;
        unitDamage = character.damage;
        unitMaxHP = character.maxHP;
        unitCurrHP = character.currHP;
        unitSpeed = character.speed;    
        unitSprite = character.sprite;

        //Deberiamos crear un spriteRenderer primero, pero no funciona
        this.GetComponent<SpriteRenderer>().sprite = unitSprite;
        sombraPrefab = GameObject.Find("sombra");

        //creamos un collider debajo del objeto
        coll = gameObject.AddComponent<CapsuleCollider2D>();    
        coll.size = new Vector2(0.2f, 0.08f);
        coll.offset = new Vector2(0, -0.14f);
        coll.direction = (CapsuleDirection2D)2;
        if (unitNames == "Geno")
        {
            coll.isTrigger = true;
        }

        unitBattleAnimator = character.battleAnimator;

        
        //creamos una sombra
        GameObject sombraRef = Instantiate(sombraPrefab, transform.position, Quaternion.identity);
        sombraRef.transform.parent = gameObject.transform;
        sombraRef.name = "sombra" + unitNames;
        auxiliar = sombraRef.transform.position;
		auxiliar.y -= 0.14f;
		auxiliar.z = 1f;
		sombraRef.transform.position = auxiliar;
        sombraRef.AddComponent<shadow>();
    }

    public bool TakeDamage(int dmg)
	{
        unitCurrHP -= dmg;
        if (unitCurrHP <= 0) return true;
        else return false;
	}

    public void Die()
    {
        Destroy(gameObject);
    }
} 
