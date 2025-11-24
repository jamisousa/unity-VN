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

        VNManager.instance.LoadFile(filePath);

    }
}
