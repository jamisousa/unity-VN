using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

namespace TESTING
{
    public class TestParsing : MonoBehaviour
    {
        [SerializeField] private TextAsset fileName;
        void Start()
        {
            SendFileToParse();

            //string line = "Speaker \"Dialogue goes in here\" Command(arguments here)";
            //string line = "Speaker \"Dialogue \\\"Goes in\\\" here! \" Command(arguments here)";
            //DialogueParser.Parse(line);
        }

        void SendFileToParse()
        {
            List<string> lines = FileManager.ReadTextAsset(fileName);

            foreach(string line in lines)
            {
               if (line == string.Empty) continue;

               DIALOGUE_LINES dl = DialogueParser.Parse(line);
               Debug.Log(dl);
            }

        }
    }

}
