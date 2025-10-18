using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

//testing script to read dialogue lines from a text file and start a conversation
public class TestDialogueFiles : MonoBehaviour
{
    [SerializeField] private TextAsset fileToRead = null;
    void Start()
    {
        StartConversation();

        //string line = "Speaker \"Dialogue goes in here\" Command(arguments here)";
        //string line = "Speaker \"Dialogue \\\"Goes in\\\" here! \" Command(arguments here)";
        //DialogueParser.Parse(line);
    }

    void StartConversation()
    {
        List<string> lines = FileManager.ReadTextAsset(fileToRead);

        //foreach (string line in lines)
        //{
        //    if (string.IsNullOrEmpty(line)) return;

        //    Debug.Log($"Segmenting line '{line}'");
        //    DIALOGUE_LINES dlLine = DialogueParser.Parse(line);

        //    int i = 0;
        //    foreach (DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment in dlLine.dialogue.segments)
        //    {
        //        Debug.Log($"Segment [{i++}] = '{segment.dialogue}' [signal={segment.startSignal.ToString()}{(segment.signalDelay > 0 ? $" {segment.signalDelay}" : $"")}]");
        //    }
        //}

        DialogueSystem.instance.Say(lines);

    }
}
