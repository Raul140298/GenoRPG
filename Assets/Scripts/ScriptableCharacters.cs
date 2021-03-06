﻿using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class ScriptableCharacters : ScriptableObject
{
    public string names;
    public int lvl, damage, maxHP, currHP, maxMP, currMP, expNeed, expGiven, currExp, probRun;
    public float speed;
    public Sprite sprite;
    public RuntimeAnimatorController battleAnimator;   
    public ParticleSystem [] attackParticle;
}
