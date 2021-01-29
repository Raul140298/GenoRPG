using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class ScriptableCharacters : ScriptableObject
{
    public string names;
    public int lvl, damage, maxHP, currHP, maxMP, currMP;
    public float speed;
    public Sprite sprite;
    public AnimatorController battleAnimator;   
    public ParticleSystem attackParticle;
}
