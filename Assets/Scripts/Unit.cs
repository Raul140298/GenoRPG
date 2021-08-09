using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;

public enum SceneState { ADVENTURESCENE, BATTLESCENE}

public class Unit : MonoBehaviour
{
    public ScriptableCharacters character;
    public string unitNames;
    public int unitLvl, unitDamage, unitMaxHP, unitCurrHP, unitMaxMP, unitCurrMP, unitExpNeed, unitExpGiven, unitCurrExp, unitProbRun;
    public float unitSpeed;
    public Sprite unitSprite;
    public GameObject sombraPrefab;
    public RuntimeAnimatorController unitBattleAnimator;
    public CameraScript cam;
    public ParticleSystem unitAttackParticle, unitSpecialParticle;
    CapsuleCollider2D coll;
    public EnemyMoveScript enemyMove;
    public ObjectPathMovementScript pathMovement;

    void Start()
    {
        //buscamos la cámara
        cam = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        //extrae la info de los scriptables characters
        unitNames = character.names;
        unitLvl = character.lvl;
        unitDamage = character.damage;
        unitMaxHP = character.maxHP;
        unitCurrHP = character.currHP;
        unitMaxMP = character.maxMP;
        unitCurrMP = character.currMP;
        unitSpeed = character.speed;    
        unitSprite = character.sprite;
        unitBattleAnimator = character.battleAnimator;//Asignamos el animator
        unitAttackParticle = character.attackParticle[0];//Asignamos las partículas de ataque
        unitSpecialParticle = character.attackParticle[1];//Asignamos las partículas del especial
        //Buscamos la sombra
        sombraPrefab = GameObject.Find("sombra");
        //Instansiamos la sombra
        GameObject sombraRef = Instantiate(sombraPrefab, transform.position, Quaternion.identity);
        sombraRef.transform.parent = gameObject.transform;
        sombraRef.name = "sombra" + unitNames;
        Vector3 auxiliar;
        auxiliar = sombraRef.transform.position;
        auxiliar.y -= 0.14f;
        auxiliar.z = 1f;
        sombraRef.transform.position = auxiliar;
        sombraRef.AddComponent<shadow>();

        //Creamos un collider debajo del objeto
        coll = gameObject.AddComponent<CapsuleCollider2D>();
        coll.size = new Vector2(0.2f, 0.08f);
        coll.offset = new Vector2(0, -0.14f);
        coll.direction = (CapsuleDirection2D)2;
        if (unitNames == "Geno")
        {
            coll.isTrigger = true;
            unitExpNeed = character.expNeed;
            //Esto de arriba no debería hacerse. Solamente debería necesiar el nivel para hallar la exp necesaria.
            //Formula para hallar unitExpNeed con el nivel
            unitCurrExp = character.currExp;
        }
		else
		{
            gameObject.AddComponent<SpriteRenderer>();
            gameObject.AddComponent<Animator>();
            gameObject.GetComponent<Animator>().runtimeAnimatorController = unitBattleAnimator;
            unitExpGiven = character.expGiven;
            unitProbRun = character.probRun;
            pathMovement = GetComponent<ObjectPathMovementScript>();

            if (!gameObject.name.Contains("bowsette"))
            {
                enemyMove = gameObject.GetComponent<EnemyMoveScript>();
                enemyMove.anim = GetComponent<Animator>();
                
            }
            if (gameObject.name.Contains("bones"))
			{
                enemyMove.anim.SetBool("isBattle", false);
                enemyMove.anim.SetBool("walking", false);
			}
            if (gameObject.name.Contains("superfireball"))
            {
            }
            

            
        }
        this.GetComponent<SpriteRenderer>().sprite = unitSprite;

        
      
    }

	public bool TakeDamage(int dmg)
	{
        unitCurrHP -= dmg;
        if (unitCurrHP <= 0) return true;
        else return false;
	}

    public bool TakeMana(int mana)
	{
        unitCurrMP -= mana;
        if (unitCurrMP <= 0) return true;
        else return false;
    }

    public void setExp(int exp)
    {
        unitCurrExp += exp;
        if (unitCurrExp == unitExpNeed)
        {
            unitLvl += 1;
            unitCurrExp = 0;
            //Formula para hallar unitExpNeed con el nivel
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        //print(unitNames + " se escondió\n");
        cam.removeUnitToObjetosEnCamara(this.gameObject);
    }

    // ... y habilítelo de nuevo cuando sea visible.
    void OnBecameVisible()
    {
        //print(unitNames + " es visible\n");
        cam.addUnitToObjetosEnCamara(this.gameObject);
    }
} 
