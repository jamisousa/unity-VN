using System.Collections;
using System.Collections.Generic;
using CHARACTERS;
using UnityEngine;

public class AudioTesting : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Running());
    }

    Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

    IEnumerator Running()
    {
        Character_Sprite Raelin = CreateCharacter("Raelin") as Character_Sprite;
        Raelin.Show();

        yield return new WaitForSeconds(2);

        //AudioManager.instance.PlaySoundEffect("Audio/SFX/RadioStatic", loop: false);

        AudioManager.instance.PlayTrack("Audio/Music/Calm", startingVolume: 0.05f, volumeCap: 0.05f, pitch: 0.7f);
        Raelin.Animate("Hop");

        yield return new WaitForSeconds(4);

        AudioManager.instance.PlayTrack("Audio/Music/Upbeat", startingVolume: 0.05f, volumeCap: 0.05f);



        //AudioManager.instance.StopSoundEffect("RadioStatic");
    }

}
