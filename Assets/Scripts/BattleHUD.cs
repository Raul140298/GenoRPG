using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text nameText;
    public Slider hpSlider, mpSlider;
    public TextMeshPro levelText;

    public void SetHUD(Unit unit)
	{
        //nameText.text = unit.unitNames;
        //levelText.text = "LVL: " + unit.unitLvl;
        //maxHP = unit.unitMaxHP;
        //currHP = unit.unitCurrHP;
        //HP.text = currHP.ToString() + '/' + maxHP.ToString();
        hpSlider.maxValue = unit.unitMaxHP;
        hpSlider.value = unit.unitCurrHP;
        mpSlider.maxValue = unit.unitMaxMP;
        mpSlider.value = unit.unitCurrMP;

    }

    public void SetHP(int hp)
	{
        //currHP = hp;
        //HP.text = currHP.ToString() + '/' + maxHP.ToString();
        hpSlider.value = hp;
        levelText.text = hp.ToString();
    }
    public void SetMP(int mp)
	{
        //currHP = hp;
        //HP.text = currHP.ToString() + '/' + maxHP.ToString();
        mpSlider.value = mp;
    }
}
