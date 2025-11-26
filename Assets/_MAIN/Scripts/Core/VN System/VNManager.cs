using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

namespace VISUALNOVEL
{

    //handles the VN startup and loading operations
    public class VNManager : MonoBehaviour
    {

        public Camera mainCamera;

        public static VNManager instance { get; private set; }

        private void Awake()
        {
            instance = this;

            VNDatabaseLinkSetup linkSetup = GetComponent<VNDatabaseLinkSetup>();
            linkSetup.SetupExternalLinks();

            VNGameSave.activeFile = new VNGameSave();
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
}