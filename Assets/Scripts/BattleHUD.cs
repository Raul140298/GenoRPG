using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text nameText;
    public Text levelText;
    public Slider hpSlider;
    int maxHP;
    int currHP;

    public void SetHUD(Unit unit)
	{
        //nameText.text = unit.unitNames;
        //levelText.text = "LVL: " + unit.unitLvl;
        //maxHP = unit.unitMaxHP;
        //currHP = unit.unitCurrHP;
        //HP.text = currHP.ToString() + '/' + maxHP.ToString();
        hpSlider.maxValue = unit.unitMaxHP;
        hpSlider.value = unit.unitCurrHP;
    }

    public void SetHP(int hp)
	{
        //currHP = hp;
        //HP.text = currHP.ToString() + '/' + maxHP.ToString();
        hpSlider.value = hp;
    }
}
