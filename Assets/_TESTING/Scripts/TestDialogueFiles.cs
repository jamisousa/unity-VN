using COMMANDS;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;
using UnityEditor;
using System.IO;
using VISUALNOVEL;

//testing script to read dialogue lines from a text file and start a conversation
public class TestDialogueFiles : MonoBehaviour
{


    [SerializeField] private TextAsset fileToRead = null;
    void Start()
    {
        StartConversation();
    }

    void StartConversation()
    {

        string fullPath = AssetDatabase.GetAssetPath(fileToRead);

        int resourcesIndex = fullPath.IndexOf("Resources/");

        string relativePath = fullPath.Substring(resourcesIndex + 10);


        string filePath = Path.ChangeExtension(relativePath, null);

        LoadFile(filePath);

    }

    public void LoadFile(string filePath)
    {
        List<string> lines = new List<string>();
        TextAsset file = Resources.Load<TextAsset>(filePath);

        try
        {
            lines = FileManager.ReadTextAsset(file);
        }
        catch
        {
            Debug.LogError($"Dialogue file at path {filePath} does not exist.");
            return;
        }

        DialogueSystem.instance.Say(lines, filePath);
    }
}
