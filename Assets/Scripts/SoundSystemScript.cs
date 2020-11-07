using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystemScript : MonoBehaviour
{
    public static AudioClip jumpSound, battleStartSound, forestMazeSoundtrack, battleSoundtrackMonsters;
    static AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        jumpSound = Resources.Load<AudioClip>("Sound_jump");
        battleStartSound = Resources.Load<AudioClip>("Sound_battle_start");
        battleSoundtrackMonsters = Resources.Load<AudioClip>("Soundtrack_battle_monsters");
        forestMazeSoundtrack = Resources.Load<AudioClip>("Soundtrack_forest_maze");

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
                audioSrc.PlayOneShot(battleSoundtrackMonsters);
                break;
        }
    }

    public static void Stop()
    {
        audioSrc.Stop();
    }
}
