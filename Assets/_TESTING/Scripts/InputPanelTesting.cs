using System.Collections;
using System.Collections.Generic;
using CHARACTERS;
using UnityEngine;

public class InputPanelTesting : MonoBehaviour
{
    public InputPanel inputPanel;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Running());
    }

    IEnumerator Running()
    {
        Character Raelin = CharacterManager.instance.CreateCharacter("Raelin", revealAfterCreation: true);

        yield return Raelin.Say("Whats your name?");

        inputPanel.Show("What is your name?");

        while (inputPanel.isWaitingOnUserInput) {

            yield return null;
        }

        string characterName = inputPanel.lastInput;

        yield return Raelin.Say($"Nice name {characterName}");

    }
}
