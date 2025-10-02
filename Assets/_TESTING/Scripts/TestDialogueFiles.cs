using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

public class TestDialogueFiles : MonoBehaviour
{
    [SerializeField] private TextAsset fileName;
    void Start()
    {
        StartConversation();

        //string line = "Speaker \"Dialogue goes in here\" Command(arguments here)";
        //string line = "Speaker \"Dialogue \\\"Goes in\\\" here! \" Command(arguments here)";
        //DialogueParser.Parse(line);
    }

    void StartConversation()
    {
        List<string> lines = FileManager.ReadTextAsset(fileName);

        DialogueSystem.instance.Say(lines);

   

    }
}
