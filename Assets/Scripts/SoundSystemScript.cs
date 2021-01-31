using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystemScript : MonoBehaviour
{
    public static AudioClip deadSound, buttonSound, jumpSound, battleStartSound, winSound;
    public static AudioClip forestMazeSoundtrack, battleMonstersSoundtrack, battleBossesSoundtrack;
    public static AudioClip genoAttackSound;
    static AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        //SOUNDS
        buttonSound = Resources.Load<AudioClip>("Sound_button");
        jumpSound = Resources.Load<AudioClip>("Sound_jump");
        deadSound = Resources.Load<AudioClip>("Sound_dead");
        genoAttackSound = Resources.Load<AudioClip>("Sound_geno_fingershot");
        battleStartSound = Resources.Load<AudioClip>("Sound_battle_start");
        winSound = Resources.Load<AudioClip>("Sound_win");

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
            case "Sound_win":
                audioSrc.PlayOneShot(winSound);
                break;
        }
	}

    public static void PlaySoundtrack(string clip2)
    {
        switch (clip2)
        {
            case "Soundtrack_forest_maze":
                audioSrc.Stop();
                audioSrc.PlayOneShot(forestMazeSoundtrack);
                break;
            case "Soundtrack_battle_monsters":
                audioSrc.Stop();
                audioSrc.PlayOneShot(battleMonstersSoundtrack);
                break;
            case "Soundtrack_battle_bosses":
                audioSrc.Stop();
                audioSrc.PlayOneShot(battleBossesSoundtrack);
                break;
        }
    }

    public static void Stop()
    {
        audioSrc.Stop();
    }
}
