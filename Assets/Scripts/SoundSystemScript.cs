using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystemScript : MonoBehaviour
{
    public static AudioClip deadSound, buttonSound, jumpSound, battleStartSound, winSound, pipeSound, itemSound, hitSound, flameSound;
    public static AudioClip forestMazeSoundtrack, battleMonstersSoundtrack, battleBossesSoundtrack;
    public static AudioClip genoAttackSound, genoSpecialSound;
    static AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        //SOUNDS
        buttonSound = Resources.Load<AudioClip>("Sound_button");
        jumpSound = Resources.Load<AudioClip>("Sound_jump");
        deadSound = Resources.Load<AudioClip>("Sound_dead");
        genoAttackSound = Resources.Load<AudioClip>("Sound_geno_fingershot");
        genoSpecialSound = Resources.Load<AudioClip>("Sound_geno_laserbeam");
        battleStartSound = Resources.Load<AudioClip>("Sound_battle_start");
        winSound = Resources.Load<AudioClip>("Sound_win");
        pipeSound = Resources.Load<AudioClip>("Sound_pipe");
        itemSound = Resources.Load<AudioClip>("Sound_item");
        hitSound = Resources.Load<AudioClip>("Sound_hit");
        flameSound = Resources.Load<AudioClip>("Sound_flame");

        //SOUNDTRACKS
        battleMonstersSoundtrack = Resources.Load<AudioClip>("Soundtrack_battle_monsters");
        forestMazeSoundtrack = Resources.Load<AudioClip>("Soundtrack_forest_maze");
        battleBossesSoundtrack = Resources.Load<AudioClip>("Soundtrack_battle_bosses");

        audioSrc = GetComponent<AudioSource>();
    }

    public static void PlaySound (string clip)
	{
        switch (clip)
		{
            case "Sound_jump":
                audioSrc.PlayOneShot(jumpSound);
                break;
            case "Sound_battle_start":
                audioSrc.PlayOneShot(battleStartSound);
                break;
            case "Sound_button":
                audioSrc.PlayOneShot(buttonSound);
                break;
            case "Sound_dead":
                audioSrc.PlayOneShot(deadSound);
                break;
            case "Sound_geno_fingershot":
                audioSrc.PlayOneShot(genoAttackSound);
                break;
            case "Sound_geno_laserbeam":
                audioSrc.PlayOneShot(genoSpecialSound);
                break;
            case "Sound_win":
                audioSrc.PlayOneShot(winSound);
                break;
            case "Sound_pipe":
                audioSrc.PlayOneShot(pipeSound);
                break;
            case "Sound_item":
                audioSrc.PlayOneShot(itemSound);
                break;
            case "Sound_hit":
                audioSrc.PlayOneShot(hitSound);
                break;
            case "Sound_flame":
                audioSrc.PlayOneShot(flameSound);
                break;
        }
	}

    public static void PlaySoundtrack(string clip2)
    {
        audioSrc.loop = true;
        audioSrc.Stop();
        switch (clip2)
        {
            case "Soundtrack_forest_maze":              
                audioSrc.clip = forestMazeSoundtrack;   
                break;
            case "Soundtrack_battle_monsters":
                audioSrc.clip = battleMonstersSoundtrack;
                break;
            case "Soundtrack_battle_bosses":
                audioSrc.clip = battleBossesSoundtrack;
                break;
        }
        audioSrc.Play();
    }

    public static void Stop()
    {
        audioSrc.Stop();
    }
}
